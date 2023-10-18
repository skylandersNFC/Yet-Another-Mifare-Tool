using System.Diagnostics;
using System.Text;
using YetAnotherMifareTool.Utils;

namespace YetAnotherMifareTool.ACR
{
    internal class CardReader : IDisposable
    {
        public delegate void delOnLogging(object sender, string e);
        public event delOnLogging OnLogging;

        private const int BLOCK_SIZE = 16;
        private const int BLOCKS_PER_SECTOR = 4;
        
        private int hContext;
        private string reader;
        private Card mCard;
       
        public CardReader()
        {
            reader = GetReaders().FirstOrDefault();
            mCard = new Card(hContext, reader);
        }

        public async Task<string> GetFirmware()
        {
            return await mCard.GetFirmware();
        }

        public async Task<byte[]> DirectCommand(byte[] cmd)
        {
            return await mCard.DirectCommand(cmd);
        }

        public async Task<byte[]> GetUid()
        {
            byte[] uid = null;
            byte loop = 0;

            Log(this, string.Format("Getting Uid..."));

            do
            {
                uid = await mCard.GetUid();
                loop++;

                if (uid == null)
                {
                    mCard.Dispose();
                    mCard = new Card(hContext, reader);

                    await Task.Delay(500);
                }
            }
            while (uid == null && loop < 10);

            if (uid != null)
                Log(this, string.Format("Uid: " + BitConverter.ToString(uid).Replace("-", "")));
            else
                Log(this, string.Format("Error: Uid is null!"));

            return uid;
        }

        public async Task<bool> UidEquals(byte[] data)
        {
            var uidCard = await GetUid();

            var uid = new byte[4];
            Buffer.BlockCopy(data, 0, uid, 0, uid.Length);

            bool equals = uidCard != null && uidCard.SequenceEqual(uid);
            return equals;
        }

        public async Task<byte[]> Read(byte[] keys)
        {
            Log(this, string.Format("Reading..."));

            byte[] input = new byte[1024];

            for (byte sector = 0; sector < 16; sector++)
            {
                int keyIndex = (BLOCK_SIZE * (BLOCKS_PER_SECTOR - 1)) + (sector * BLOCK_SIZE * BLOCKS_PER_SECTOR);
                var key = new byte[6];
                Buffer.BlockCopy(keys, keyIndex, key, 0, key.Length);

                var buffer = await ReadSector(sector, key);
                if (buffer == null)
                {
                    input = null;
                    break;
                }

                Buffer.BlockCopy(buffer, 0, input, sector * buffer.Length, buffer.Length);
            }

            if (input != null)
                Log(this, string.Format("Done reading."));

            return input;
        }

        private async Task<byte[]> ReadSector(int sector, byte[] key)
        {
            byte[] sectorBytes = new byte[BLOCK_SIZE * BLOCKS_PER_SECTOR];

            if (await mCard.Login(sector, key, GeneralAuthenticateCommand.GeneralAuthenticateKeyType.MifareKeyA))
            {
                Log(this, string.Format("Authenticate successful."));

                for (int block = 0; block < BLOCKS_PER_SECTOR; block++)
                {
                    var res = await mCard.Read(sector, block);
                    if (res.Item1)
                    {
                        Buffer.BlockCopy(res.Item2, 0, sectorBytes, block * BLOCK_SIZE, BLOCK_SIZE);
                        Log(this, string.Format("Read sector {0:X2}, block {1:X2}", sector, block));
                    }
                    else
                    {
                        Log(this, string.Format("Error in sector {0:X2}, block {1:X2}", sector, block));
                        return null;
                    }
                }

                // copy key to dump
                Buffer.BlockCopy(key, 0, sectorBytes, sectorBytes.Length - BLOCK_SIZE, key.Length);

                Log(this, string.Format("Read sector {0:X2}", sector));
            }
            else
            {
                Log(this, string.Format("Error while authenticating!"));
            }

            return sectorBytes;
        }

        public async Task<byte[]> ReadBlockZero()
        {
            if (await mCard.Login(0, Constants.SECTOR_ZERO_KEY, GeneralAuthenticateCommand.GeneralAuthenticateKeyType.MifareKeyA) ||
                await mCard.Login(0, Constants.FACTORY_KEY, GeneralAuthenticateCommand.GeneralAuthenticateKeyType.MifareKeyA))
            {
                var res = await mCard.Read(0, 0);
                if (res.Item1)
                {
                    byte[] blockBytes = new byte[BLOCK_SIZE];
                    Buffer.BlockCopy(res.Item2, 0, blockBytes, 0, blockBytes.Length);

                    return blockBytes;
                }
                else
                {
                    Log(this, string.Format("Error in sector {0:X2}, block {1:X2}", 0, 0));
                }
            }
            else
            {
                Log(this, string.Format("Error while authenticating!"));
            }

            return null;
        }

        public async Task<bool> Write(byte[] data, bool writeBlockZero)
        {
            Log(this, string.Format("Writing..."));

            bool success = false;

            for (byte sector = 0; sector < 16; sector++)
            {
                int keyIndex = (BLOCK_SIZE * (BLOCKS_PER_SECTOR - 1)) + (sector * BLOCK_SIZE * BLOCKS_PER_SECTOR);
                var key = new byte[6];
                Buffer.BlockCopy(data, keyIndex, key, 0, key.Length);

                var buffer = new byte[BLOCK_SIZE * BLOCKS_PER_SECTOR];
                Buffer.BlockCopy(data, sector * buffer.Length, buffer, 0, buffer.Length);

                success = await WriteSector(sector, key, buffer, writeBlockZero);
                if (!success)
                {
                    break;
                }
            }

            if (success)
                Log(this, string.Format("Done writing."));

            return success;
        }

        private async Task<bool> WriteSector(int sector, byte[] key, byte[] data, bool writeBlockZero)
        {
            if (await mCard.Login(sector, key, GeneralAuthenticateCommand.GeneralAuthenticateKeyType.MifareKeyA) ||
                await mCard.Login(sector, Constants.FACTORY_KEY, GeneralAuthenticateCommand.GeneralAuthenticateKeyType.MifareKeyA))
            {
                Log(this, string.Format("Authenticate successful."));

                byte startBlock = sector == 0 ? (writeBlockZero ? (byte)0 : (byte)1) : (byte)0;
                for (byte block = startBlock; block < BLOCKS_PER_SECTOR; block++)
                {
                    byte[] buffer = new byte[BLOCK_SIZE];
                    Buffer.BlockCopy(data, block * BLOCK_SIZE, buffer, 0, buffer.Length);

                    var res = await mCard.Write(sector, block, buffer);
                    if (res)
                    {
                        Log(this, string.Format("Write sector {0:X2}, block {1:X2}", sector, block));
                    }
                    else
                    if (block == 3)
                    {
                        // ignore errors in sector trailer
                    }
                    else
                    {
                        Log(this, string.Format("Error in sector {0:X2}, block {1:X2}", sector, block));
                        return false;
                    }
                }
                Log(this, string.Format("Write sector {0:X2}", sector));

                return true;
            }
            else
            {
                Log(this, string.Format("Error while authenticating!"));
                return false;
            }
        }

        private List<string> GetReaders()
        {
            int retCode = 0;
            try
            {
                retCode = ModWinsCard.SCardEstablishContext(ModWinsCard.SCARD_SCOPE_USER, 0, 0, ref hContext);

                int readersLength = 0;
                retCode = ModWinsCard.SCardListReaders(hContext, null, null, ref readersLength);

                byte[] readerBytes = new byte[readersLength];
                retCode = ModWinsCard.SCardListReaders(hContext, null, readerBytes, ref readersLength);

                var readers = Encoding.UTF8.GetString(readerBytes, 0, readerBytes.Length);
                return new List<string>(readers.Split('\0'))
                    .Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine("RetCode: " + retCode);

                return null;
            }
        }

        private void Log(object sender, string e)
        {
            if (OnLogging != null)
                OnLogging(sender, e);
        }

        #region Disposing

        private void Dispose(bool disposing)
        {
            if (mCard != null)
            {
                mCard.Dispose();
                mCard = null;
            }
        }

        public void Dispose()
        {
            Debug.WriteLine("Dispose: CardConnection");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~CardReader()
        {
            Dispose(false);
        }

        #endregion
    }
}

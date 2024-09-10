using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            Log(this, "Getting Uid...");

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
                Log(this, $"Uid: {BitConverter.ToString(uid).Replace("-", "")}");
            else
                Log(this, "Error: Uid is null!");

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
            Log(this, "Reading...");

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
                Log(this, "Done reading.");

            return input;
        }

        private async Task<byte[]> ReadSector(int sector, byte[] key)
        {
            byte[] sectorBytes = new byte[BLOCK_SIZE * BLOCKS_PER_SECTOR];

            Log(this, $"Reading sector {sector}...");

            if (await mCard.Login(sector, key, GeneralAuthenticateCommand.GeneralAuthenticateKeyType.MifareKeyA))
            {
                Log(this, $"Sector {sector} authenticated successfully.");

                for (int block = 0; block < BLOCKS_PER_SECTOR; block++)
                {
                    var res = await mCard.Read(sector, block);
                    if (res.Item1)
                    {
                        Buffer.BlockCopy(res.Item2, 0, sectorBytes, block * BLOCK_SIZE, BLOCK_SIZE);
                        //Log(this, $"Read sector {sector}, block {block} successfully.");
                    }
                    else
                    {
                        Log(this, $"Error in sector {sector}, block {block}!");
                        return null;
                    }
                }

                // copy key to dump
                Buffer.BlockCopy(key, 0, sectorBytes, sectorBytes.Length - BLOCK_SIZE, key.Length);

                Log(this, $"Sector {sector} read successfully.");
            }
            else
            {
                Log(this, $"Error while authenticating sector {sector}!");
            }

            return sectorBytes;
        }

        public async Task<byte[]> ReadManufacturerBlock()
        {
            Log(this, "Reading manufacturer block...");

            if (await mCard.Login(0, Constants.SECTOR_ZERO_KEY, GeneralAuthenticateCommand.GeneralAuthenticateKeyType.MifareKeyA) ||
                await mCard.Login(0, Constants.FACTORY_KEY, GeneralAuthenticateCommand.GeneralAuthenticateKeyType.MifareKeyA))
            {
                var res = await mCard.Read(0, 0);
                if (res.Item1)
                {
                    byte[] blockBytes = new byte[BLOCK_SIZE];
                    Buffer.BlockCopy(res.Item2, 0, blockBytes, 0, blockBytes.Length);

                    Log(this, "Manufacturer block read successfully.");

                    return blockBytes;
                }
                else
                {
                    Log(this, "Error manufacturer block!");
                }
            }
            else
            {
                Log(this, $"Error while authenticating sector 0x00!");
            }

            return null;
        }

        public async Task<bool> Write(byte[][] keys, byte[] data, bool writeManufacturerBlock)
        {
            Log(this, "Writing...");

            bool success = false;

            for (byte sector = 0; sector < 16; sector++)
            {
                var buffer = new byte[BLOCK_SIZE * BLOCKS_PER_SECTOR];
                Buffer.BlockCopy(data, sector * buffer.Length, buffer, 0, buffer.Length);

                success = await WriteSector(sector, keys[sector], buffer);
                if (!success)
                {
                    break;
                }
            }

            if (success && writeManufacturerBlock)
            {
                var buffer = new byte[BLOCK_SIZE];
                Buffer.BlockCopy(data, 0, buffer, 0, buffer.Length);

                success = await WriteManufacturerBlock(keys[0], buffer);
            }

            if (success)
                Log(this, "Done writing.");

            return success;
        }

        private async Task<bool> WriteSector(int sector, byte[] key, byte[] data)
        {
            if (await mCard.Login(sector, key, GeneralAuthenticateCommand.GeneralAuthenticateKeyType.MifareKeyA) ||
                await mCard.Login(sector, Constants.FACTORY_KEY, GeneralAuthenticateCommand.GeneralAuthenticateKeyType.MifareKeyA))
            {
                Log(this, $"Sector {sector} authenticated successfully.");

                byte startBlock = sector == 0 ? (byte)1 : (byte)0;
                for (byte block = startBlock; block < BLOCKS_PER_SECTOR; block++)
                {
                    byte[] buffer = new byte[BLOCK_SIZE];
                    Buffer.BlockCopy(data, block * BLOCK_SIZE, buffer, 0, buffer.Length);

                    var res = await mCard.Write(sector, block, buffer);
                    if (res)
                    {
                        Log(this, $"Sector {sector}, block {block} written successfully.");
                    }
                    else
                    if (block == 3)
                    {
                        // ignore errors in sector trailer
                    }
                    else
                    {
                        Log(this, $"Error in sector {sector}, block {block}!");
                        return false;
                    }
                }
                Log(this, $"Sector {sector} written successfully.");

                return true;
            }
            else
            {
                Log(this, $"Error while authenticating sector {sector}!");
                return false;
            }
        }

        private async Task<bool> WriteManufacturerBlock(byte[] key, byte[] data)
        {
            if (await mCard.Login(0, key, GeneralAuthenticateCommand.GeneralAuthenticateKeyType.MifareKeyA) ||
                await mCard.Login(0, Constants.FACTORY_KEY, GeneralAuthenticateCommand.GeneralAuthenticateKeyType.MifareKeyA))
            {
                Log(this, "Manufacturer block authenticated successfully.");

                var res = await mCard.Write(0, 0, data);
                if (res)
                {
                    Log(this, $"Manufacturer block written successfully.");
                }
                else
                {
                    Log(this, $"Error in manufacturer block!");
                    return false;
                }

                return true;
            }
            else
            {
                Log(this, $"Error while authenticating manufacturer block!");
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
                Debug.WriteLine($"RetCode: {retCode}");

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

using System.Diagnostics;
using System.Text;

namespace YetAnotherMifareTool.ACR
{
    internal class Card : IDisposable
    {
        private int hContext;
        private string readerName;

        private CardConnection mCardConnection;
        private readonly Task initialization;

        public int ActiveSector { get; set; }

        internal Card(int hContext, string readerName)
        {
            this.hContext = hContext;
            this.readerName = readerName;

            initialization = Initialize();
        }

        private async Task Initialize()
        {
            mCardConnection = await ConnectAsync();
        }

        public Task<CardConnection> ConnectAsync()
        {
            int card = 0;
            int protocol = 0;

            int retCode = ModWinsCard.SCardConnect(hContext, readerName, ModWinsCard.SCARD_SHARE_SHARED,
                ModWinsCard.SCARD_PROTOCOL_T0 | ModWinsCard.SCARD_PROTOCOL_T1, ref card, ref protocol);

            return Task.FromResult(new CardConnection(card, protocol));
        }

        public async Task<string> GetFirmware()
        {
            var apduRes = await TransceiveAsync(new GetFirmwareCommand());
            return (apduRes.Succeeded) ? Encoding.ASCII.GetString(apduRes.ResponseData) : null;
        }

        public async Task<byte[]> GetUid()
        {
            var apduRes = await TransceiveAsync(new GetUidCommand());
            return (apduRes.Succeeded) ? apduRes.ResponseData : null;
        }

        public async Task<byte[]> DirectCommand(byte[] cmd)
        {
            var apduRes = await TransceiveAsync(new DirectCommand(cmd));
            return (apduRes.Succeeded) ? apduRes.ResponseData : null;
        }

        public async Task<bool> Login(int sector, byte[] key, GeneralAuthenticateCommand.GeneralAuthenticateKeyType keyType)
        {
            // Get the first block for the sector
            var blockNumber = SectorToBlock(sector, 0);

            // only use location 0
            byte location = 0; 

            // Load the key to the location
            var r = await TransceiveAsync(new LoadKeyCommand(key, location));
            if (!r.Succeeded)
                return false; // could not load the key

            var res = await TransceiveAsync(new AuthenticateCommand(blockNumber, location, keyType));

            return res.Succeeded;
        }

        public async Task<Tuple<bool, byte[]>> Read(int sector, int datablock)
        {
            var blockNumber = SectorToBlock(sector, datablock);

            var readRes = await TransceiveAsync(new ReadCommand(blockNumber));

            return Tuple.Create(readRes.Succeeded, readRes.ResponseData);
        }

        public async Task<bool> Write(int sector, int datablock, byte[] data)
        {
            var blockNumber = SectorToBlock(sector, datablock);

            var write = new WriteCommand(blockNumber, ref data);
            var adpuRes = await TransceiveAsync(write);

            return adpuRes.Succeeded;
        }

        private static byte SectorToBlock(int sector, int dataBlock)
        {
            if (sector >= 40 || sector < 0)
                throw new ArgumentOutOfRangeException(sector.ToString(), "sector must be between 0 and 39");

            if (dataBlock < 0)
                throw new ArgumentOutOfRangeException(dataBlock.ToString(), "value must be greater or equal to 0");

            if (sector < 32 && dataBlock >= 4)
                throw new ArgumentOutOfRangeException(dataBlock.ToString(), "Sectors 0-31 only have data blocks 0-3");
            if (dataBlock >= 16)
                throw new ArgumentOutOfRangeException(dataBlock.ToString(), "Sectors 32-39 have data blocks 0-15");

            int block;
            // first 32 sectors are 4 blocks
            // last 8 are 16 blocks
            if (sector < 32)
            {
                block = (sector * 4) + dataBlock;
            }
            else
            {
                const int startingBlock = 32 * 4; // initial block number
                var largeSectors = sector - 32; // number of 16 block sectors
                block = (largeSectors * 16) + dataBlock + startingBlock;
            }

            return (byte)block;
        }

        private async Task<ApduResponse> TransceiveAsync(ApduCommand apduCommand)
        {
            await initialization;

            return (mCardConnection != null ? mCardConnection.Transceive(apduCommand) : new NoResponse());
        }

        public void Dispose()
        {
            Debug.WriteLine("Dispose: Card");
            if (mCardConnection != null)
            {
                mCardConnection.Dispose();
                mCardConnection = null;
            }
        }
    }
}

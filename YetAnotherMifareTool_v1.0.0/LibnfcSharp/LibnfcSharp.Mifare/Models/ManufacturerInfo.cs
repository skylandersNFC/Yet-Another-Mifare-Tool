using System.Linq;

namespace LibnfcSharp.Mifare.Models
{
    public class ManufacturerInfo
    {
        public byte[] Uid { get; private set; }
        public byte Bcc { get; private set; }
        public byte Sak { get; private set; }
        public byte[] Atqa { get; private set; }
        public byte[] ManufacturerData { get; private set; }
        public byte[] RawData { get; private set; }

        private ManufacturerInfo()
        {
            Uid = new byte[4];
            Bcc = 0x00;
            Sak = 0x00;
            Atqa = new byte[2];
            ManufacturerData = new byte[8];
            RawData = new byte[16];
        }

        public ManufacturerInfo(byte[] manufacturerBlock)
            : this()
        {
            if (manufacturerBlock == null || manufacturerBlock.Length != MifareClassic.BLOCK_SIZE)
                return;

            Uid = manufacturerBlock.Take(4).ToArray();
            Bcc = manufacturerBlock[4];
            Sak = manufacturerBlock[5];
            Atqa = manufacturerBlock.Skip(6).Take(2).Reverse().ToArray();
            ManufacturerData = manufacturerBlock.Skip(6).ToArray();
            RawData = manufacturerBlock.Take(MifareClassic.BLOCK_SIZE).ToArray();
        }
    }
}

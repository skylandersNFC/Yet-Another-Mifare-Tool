using System;
using System.IO;
using YetAnotherMifareTool.Utils;

namespace YetAnotherMifareTool.Core
{
    public class Toy
    {
        public string FilePath { get; set; }
        public string Name { get { return getName(); } }
        public byte[] Data { get; set; }
        public bool IsDataValid { get { return Data != null && Data.Length == 1024; } }
        public uint Id { get { return getId(); } }
        public uint IdExt { get { return getIdExt(); } }
        public byte[] ManufacturerBlock { get { return getManufacturerBlock(); } }

        public Toy(string filePath)
        {
            if (File.Exists(filePath))
            {
                FilePath = filePath;
                Data = File.ReadAllBytes(FilePath);
            }
        }

        private uint getId()
        {
            if (this.Data == null)
                return 0;

            byte[] id = new byte[2];
            for (int i = 0; i < Constants.ID_IDXS.Length; i++)
            {
                id[i] = this.Data[Constants.ID_IDXS[i]];
            }
            return BitConverter.ToUInt16(id, 0);
        }

        private uint getIdExt()
        {
            if (this.Data == null)
                return 0;

            byte[] id = new byte[2];
            for (int i = 0; i < Constants.IDEXT_IDXS.Length; i++)
            {
                id[i] = this.Data[Constants.IDEXT_IDXS[i]];
            }
            return BitConverter.ToUInt16(id, 0);
        }

        private byte[] getManufacturerBlock()
        {
            if (this.Data == null)
                return new byte[Constants.BLOCK_SIZE];

            byte[] manufacturerBlock = new byte[Constants.BLOCK_SIZE];
            Buffer.BlockCopy(this.Data, 0, manufacturerBlock, 0, manufacturerBlock.Length);

            return manufacturerBlock;
        }

        private string getName()
        {
            if (string.IsNullOrEmpty(this.FilePath))
                return string.Format("[Generated - Id: {0:X}, IdExt: {1:X}]", this.Id, this.IdExt);
            else
                return Path.GetFileName(this.FilePath);
        }
    }
}

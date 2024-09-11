using System.IO;
using System.Linq;
using YetAnotherMifareTool.Extensions;
using YetAnotherMifareTool.Utils;

namespace YetAnotherMifareTool.Core
{
    public class Toy_old
    {
        private const byte ID_INDEX = 0x10;
        private const byte ID_SIZE = 0x02;
        private const byte VARIANT_INDEX = 0x1C;
        private const byte VARIANT_SIZE = 0x02;

        public string FilePath { get; set; }
        public string Name { get { return getName(); } }
        public byte[] Data { get; set; }
        public bool IsDataValid { get { return Data != null && Data.Length == 1024; } }
        public uint Id { get { return getId(); } }
        public uint Variant { get { return getVariant(); } }
        public byte[] ManufacturerBlock { get { return getManufacturerBlock(); } }

        public Toy_old(string filePath)
        {
            if (File.Exists(filePath))
            {
                FilePath = filePath;
                Data = File.ReadAllBytes(FilePath);
            }
        }

        private uint getId()
        {
            return Data?
                .Skip(ID_INDEX)
                .Take(ID_SIZE)
                .ToArray()
                .ToUInt16()
                ?? 0;
        }

        private uint getVariant()
        {
            return Data?
                .Skip(VARIANT_INDEX)
                .Take(VARIANT_SIZE)
                .ToArray()
                .ToUInt16()
                ?? 0;
        }

        private byte[] getManufacturerBlock()
        {
            return Data?
                .Take(Constants.BLOCK_SIZE)
                .ToArray()
                ?? new byte[Constants.BLOCK_SIZE];
        }

        private string getName()
        {
            if (string.IsNullOrEmpty(FilePath))
                return string.Format("[Generated - Id: {0:X}, Variant: {1:X}]", Id, Variant);
            else
                return Path.GetFileName(FilePath);
        }
    }
}

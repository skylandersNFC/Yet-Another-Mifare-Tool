using LibnfcSharp.Mifare;
using System.IO;
using System.Linq;
using YetAnotherMifareTool.Extensions;

namespace YetAnotherMifareTool.Models
{
    internal class DumpFile : Toy
    {
        private const byte ID_SIZE = 0x02;
        private const byte ID_OFFSET = 0x10;
        private const byte VARIANT_SIZE = 0x02;
        private const byte VARIANT_OFFSET = 0x1C;

        public string FilePath { get; set; }
        public bool IsValid { get { return Data != null && Data.Length == 1024; } }

        public DumpFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                FilePath = filePath;
                Data = File.ReadAllBytes(FilePath);
                Name = Path.GetFileName(FilePath);
                ManufacturerBlock = Data.Take(MifareClassic.BLOCK_SIZE).ToArray();
                Id = Data.Skip(ID_OFFSET).Take(ID_SIZE).ToArray().ToUInt16();
                Variant = Data.Skip(VARIANT_OFFSET).Take(VARIANT_SIZE).ToArray().ToUInt16();
            }
        }
    }
}

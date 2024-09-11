using System.IO;
using System.Linq;
using YetAnotherMifareTool.Extensions;
using YetAnotherMifareTool.Utils;

namespace YetAnotherMifareTool.Core
{
    internal class DumpFile : Toy
    {
        public string FilePath { get; set; }
        public bool IsValid { get { return Data != null && Data.Length == 1024; } }

        public DumpFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                FilePath = filePath;
                Data = File.ReadAllBytes(FilePath);
                Name = Path.GetFileName(FilePath);
                ManufacturerBlock = Data.Take(Constants.BLOCK_SIZE).ToArray();
                Id = Data.Skip(Constants.ID_OFFSET).Take(Constants.ID_SIZE).ToArray().ToUInt16();
                Variant = Data.Skip(Constants.VARIANT_OFFSET).Take(Constants.VARIANT_SIZE).ToArray().ToUInt16();
            }
        }
    }
}

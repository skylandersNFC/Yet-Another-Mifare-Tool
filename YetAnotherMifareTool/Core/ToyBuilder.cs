using System;
using System.Linq;
using YetAnotherMifareTool.Extensions;
using YetAnotherMifareTool.Utils;

namespace YetAnotherMifareTool.Core
{
    internal class ToyBuilder
    {
        private Toy _toy;

        private bool _withRecalculatedKeys = false;
        private bool _withUnlockedAccessConditions = false;

        public ToyBuilder()
        {
            _toy = new Toy();
        }

        public ToyBuilder WithManufacturerBlock(byte[] manufacturerBlock)
        {
            _toy.ManufacturerBlock = manufacturerBlock;
            return this;
        }

        public ToyBuilder WithId(ushort id)
        {
            _toy.Id = id;
            return this;
        }

        public ToyBuilder WithVariant(ushort variant)
        {
            _toy.Variant = variant;
            return this;
        }

        public ToyBuilder WithRecalculatedKeys()
        {
            _withRecalculatedKeys = true;
            return this;
        }

        public ToyBuilder WithUnlockedAccessConditions()
        {
            _withUnlockedAccessConditions = true;
            return this;
        }

        public Toy BuildFromScratch()
        {
            _toy.Data = new byte[1024];

            Array.Copy(_toy.ManufacturerBlock, 0, _toy.Data, 0, _toy.ManufacturerBlock.Length);

            byte[] data_id = _toy.Id.ToBytes();
            byte[] data_variant = _toy.Variant.ToBytes();

            _toy.Data[17] = data_id[1];
            _toy.Data[16] = data_id[0];
            _toy.Data[29] = data_variant[1];
            _toy.Data[28] = data_variant[0];
            byte[] crcZero = Crypto.ComputeCRC16(_toy.Data, 30).ToBytes();
            _toy.Data[30] = crcZero[0];
            _toy.Data[31] = crcZero[1];

            if (_withRecalculatedKeys) _toy.Data = Magic.AddRecalculatedKeys(_toy.Data);
            if (_withUnlockedAccessConditions) _toy.Data = Magic.UnlockedAccessConditions(_toy.Data);

            return _toy;
        }

        public Toy BuildFromDumpFile(DumpFile dumpFile)
        {
            _toy.Data = dumpFile.Data.Take(1024).ToArray();
            _toy.Name = dumpFile.Name;
            _toy.ManufacturerBlock = dumpFile.Data.Take(Constants.BLOCK_SIZE).ToArray();
            _toy.Id = dumpFile.Id;
            _toy.Variant = dumpFile.Variant;

            if (_withRecalculatedKeys) _toy.Data = Magic.AddRecalculatedKeys(_toy.Data);
            if (_withUnlockedAccessConditions) _toy.Data = Magic.UnlockedAccessConditions(_toy.Data);

            return _toy;
        }
    }
}

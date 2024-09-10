using System;
using System.Threading.Tasks;
using YetAnotherMifareTool.ACR;

namespace YetAnotherMifareTool.Core
{
    public class ToyFactory : IDisposable
    {
        private CardReader _cardReader;

        public event EventHandler<string> OnLogging;

        public ToyFactory()
        {
            _cardReader = new CardReader();

            _cardReader.OnLogging += (sender, e) =>
            {
                if (OnLogging != null && e != null)
                {
                    OnLogging(sender, e);
                }
            };
        }

        public async Task<byte[]> GetUid()
        {
            var uid = await _cardReader.GetUid();
            return uid;
        }

        public async Task<byte[]> ReadManufacturerBlock()
        {
            var manufacturerBlock = await _cardReader.ReadManufacturerBlock();
            return manufacturerBlock;
        }

        public async Task Write(byte[][] keys, byte[] data, bool writeManufacturerBlock)
        {
            await _cardReader.Write(keys, data, writeManufacturerBlock);
        }

        public void Dispose()
        {
            _cardReader?.Dispose();
        }
    }
}

using YetAnotherMifareTool.ACR;

namespace YetAnotherMifareTool.Core
{
    public class ToyFactory
    {
        public event EventHandler<string> OnLogging;

        public async Task<string> GetFirmware()
        {
            var cardReader = new CardReader();

            var firmware = await cardReader.GetFirmware();

            cardReader.Dispose();

            return firmware;
        }

        public async Task<byte[]> GetBlockZero()
        {
            var cardReader = new CardReader();

            var blockZero = await cardReader.ReadBlockZero();

            cardReader.Dispose();

            return blockZero;
        }

        public async Task Write(byte[] input, bool writeBlockZero)
        {
            var cardReader = new CardReader();

            cardReader.OnLogging += (sender, e) =>
            {
                if (OnLogging != null && e != null)
                {
                    OnLogging(sender, e);
                }
            };

            await cardReader.Write(input, writeBlockZero);

            cardReader.Dispose();
        }
    }
}

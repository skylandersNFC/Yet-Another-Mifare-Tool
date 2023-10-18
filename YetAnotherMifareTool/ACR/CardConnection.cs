using System.Diagnostics;

namespace YetAnotherMifareTool.ACR
{
    internal sealed class CardConnection : IDisposable
    {
        private int hCard;
        private int hProtocol;

        public CardConnection(int card, int protocol)
        {
            this.hCard = card;
            this.hProtocol = protocol;
        }

        public ApduResponse Transceive(ApduCommand apduCommand)
        {
            var apduRes = (ApduResponse)Activator.CreateInstance(apduCommand.ApduResponseType);
            var responseBuf = this.Transceive(apduCommand.GetBuffer());
            apduRes.ExtractResponse(responseBuf);

            return apduRes;
        }

        private byte[] Transceive(byte[] buffer)
        {
            if (buffer == null) throw new ArgumentNullException(buffer.ToString());

            var sioreq = new ModWinsCard.SCARD_IO_REQUEST
            {
                dwProtocol = 0x2,
                cbPciLength = 8
            };
            var rioreq = new ModWinsCard.SCARD_IO_REQUEST
            {
                cbPciLength = 8,
                dwProtocol = 0x2
            };

            var receiveBuffer = new byte[255];
            var rlen = receiveBuffer.Length;

            var retVal = ModWinsCard.SCardTransmit(hCard, ref sioreq, buffer, buffer.Length, ref rioreq, receiveBuffer, ref rlen);

            var retBuf = new byte[rlen];
            Array.Copy(receiveBuffer, retBuf, rlen);

            return retBuf;
        }

        private void Disconnect()
        {
            if (hCard != 0)
            {
                Debug.WriteLine(string.Format("SmartCardConnection.Disconnect: {0}", hCard));
                var retVal = ModWinsCard.SCardDisconnect(hCard, ModWinsCard.SCARD_UNPOWER_CARD);

                hCard = 0;
            }
        }

        private void Dispose(bool disposing)
        {
            Disconnect();
        }

        public void Dispose()
        {
            Debug.WriteLine("Dispose: CardConnection");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~CardConnection()
        {
            Dispose(false);
        }
    }
}

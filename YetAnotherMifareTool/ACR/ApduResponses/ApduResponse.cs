using System;
using System.IO;

namespace YetAnotherMifareTool.ACR
{
    public class ApduResponse
    {
        /// <summary>
        /// Class constructor
        /// </summary>
        public ApduResponse() { }
        /// <summary>
        /// methode to extract the response data, status and qualifier
        /// </summary>
        /// <param name="response"></param>
        public virtual void ExtractResponse(byte[] response)
        {
            if (response.Length < 2)
            {
                throw new InvalidOperationException("APDU response must be at least 2 bytes");
            }
            using (var reader = new MemoryStream(response))
            {
                ResponseData = new byte[response.Length - 2];
                reader.Read(ResponseData, 0, ResponseData.Length);

                SW1 = (byte)reader.ReadByte();
                SW2 = (byte)reader.ReadByte();
            }
        }
        /// <summary>
        /// Detects if the command has succeeded
        /// </summary>
        /// <returns></returns>
        public virtual bool Succeeded { get { return SW == 0x9000 || SW == 0x3130; } }

        /// <summary>
        /// command processing status
        /// </summary>
        public byte SW1 { get; set; }
        /// <summary>
        /// command processing qualifier
        /// </summary>
        public byte SW2 { get; set; }
        /// <summary>
        /// Wrapper property to read both response status and qualifer
        /// </summary>
        public ushort SW
        {
            get
            {
                return (ushort)(((ushort)SW1 << 8) | (ushort)SW2);
            }
            set
            {
                SW1 = (byte)(value >> 8);
                SW2 = (byte)(value & 0xFF);
            }
        }
        /// <summary>
        /// Response data
        /// </summary>
        public byte[] ResponseData { get; set; }
        /// <summary>
        /// Mapping response status and qualifer to human readable format
        /// </summary>
        public virtual string SWTranslation
        {
            get
            {
                switch (SW)
                {
                    case 0x9000:
                        return "Success";

                    case 0x6700:
                        return "Incorrect length or address range error";

                    case 0x6800:
                        return "The requested function is not supported by the card";

                    default:
                        return "Unknown";
                }
            }
        }
        /// <summary>
        /// Helper method to print the response in a readable format
        /// </summary>
        /// <returns>
        /// return string formatted response
        /// </returns>
        public override string ToString()
        {
            return "ApduResponse SW=" + SW.ToString("X4") + " (" + SWTranslation + ")" + ((ResponseData != null && ResponseData.Length > 0) ? (",Data=" + BitConverter.ToString(ResponseData).Replace("-", "")) : "");
        }
    }
}

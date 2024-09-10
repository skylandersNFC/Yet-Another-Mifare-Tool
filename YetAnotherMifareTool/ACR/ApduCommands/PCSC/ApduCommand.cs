using System;
using System.IO;

namespace YetAnotherMifareTool.ACR
{
    public class ApduCommand
    {
        public ApduCommand(byte cla, byte ins, byte p1, byte p2, byte[] commandData, byte? le)
        {
            if (commandData != null && commandData.Length > 254)
            {
                throw new NotImplementedException();
            }
            CLA = cla;
            INS = ins;
            P1 = p1;
            P2 = p2;
            CommandData = commandData;
            Le = le;

            ApduResponseType = typeof(ApduResponse);
        }

        public ApduCommand(byte cla, byte ins, byte p1, byte p2, byte? lc, byte[] directData)
        {
            if (directData != null && directData.Length > 254)
            {
                throw new NotImplementedException();
            }
            CLA = cla;
            INS = ins;
            P1 = p1;
            P2 = p2;
            DirectData = directData;
            Lc = lc;

            ApduResponseType = typeof(ApduResponse);
        }
        /// <summary>
        /// Class of instructions
        /// </summary>
        public byte CLA { get; set; }
        /// <summary>
        /// Instruction code
        /// </summary>
        public byte INS { get; set; }
        /// <summary>
        /// Instruction parameter 1
        /// </summary>
        public byte P1 { get; set; }
        /// <summary>
        /// Instruction parameter 2
        /// </summary>
        public byte P2 { get; set; }
        /// <summary>
        /// Maximum number of bytes expected in the response of this command
        /// </summary>
        public byte? Le { get; set; }
        /// <summary>
        /// Contiguous array of bytes representing commands data
        /// </summary>
        public byte[] CommandData { get; set; }
        /// <summary>
        /// Maximum number of bytes in data
        /// </summary>
        public byte? Lc { get; set; }
        /// <summary>
        /// Contiguous array of bytes representing data
        /// </summary>
        public byte[] DirectData { get; set; }
        /// <summary>
        /// Expected response type for this command.
        /// Provides mechanism to bind commands to responses
        /// </summary>
        public Type ApduResponseType { set; get; }
        /// <summary>
        /// Packs the current command into contiguous buffer bytes
        /// </summary>
        /// <returns>
        /// buffer holds the current wire/air format of the command
        /// </returns>
        public byte[] GetBuffer()
        {
            using (var ms = new MemoryStream())
            {
                ms.WriteByte(CLA);
                ms.WriteByte(INS);
                ms.WriteByte(P1);
                ms.WriteByte(P2);

                if (CommandData != null && CommandData.Length > 0)
                {
                    ms.WriteByte((byte)CommandData.Length);
                    ms.Write(CommandData, 0, CommandData.Length);
                }

                if (Le != null)
                {
                    ms.WriteByte((byte)Le);
                }

                if (DirectData != null && DirectData.Length > 0)
                {
                    ms.WriteByte((byte)DirectData.Length);
                    ms.Write(DirectData, 0, DirectData.Length);
                }

                if (Lc != null)
                {
                    ms.WriteByte((byte)Lc);
                }
                return ms.ToArray();
            }


        }
        /// <summary>
        /// Helper method to print the command in a readable format
        /// </summary>
        /// <returns>
        /// return string formatted command
        /// </returns>
        public override string ToString()
        {
            return "ApduCommand CLA=" + CLA.ToString("X2") + ",INS=" + INS.ToString("X2") + ",P1=" + P1.ToString("X2") + ",P2=" + P2.ToString("X2") + ((CommandData != null && CommandData.Length > 0) ? (",Data=" + BitConverter.ToString(CommandData).Replace("-", "")) : "");
        }
    }
}

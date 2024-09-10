using System;

namespace YetAnotherMifareTool.ACR
{
    public class SelectCommand : ApduCommand
    {
        public SelectCommand(byte[] aid, byte? le)
            : base((byte)Cla.CompliantCmd0x, (byte)Ins.SelectFile, 0x04, 0x00, aid, le)
        {
        }

        public byte[] AID
        {
            set { CommandData = value; }
            get { return CommandData; }
        }
        public override string ToString()
        {
            return "SelectCommand AID=" + BitConverter.ToString(CommandData).Replace("-", "");
        }
    }
}

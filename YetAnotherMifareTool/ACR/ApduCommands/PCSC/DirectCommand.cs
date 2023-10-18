namespace YetAnotherMifareTool.ACR
{
    /// <summary>
    /// PCSC Direct command
    /// </summary>
    public class DirectCommand : ApduCommand
    {
        public static readonly byte[] GetGeneralStatus = new byte[] { 0xd4, 0x04 };
        public static readonly byte[] GetPN532Firmware = new byte[] { 0xd4, 0x02 };
        public static readonly byte[] TgInitAsTarget = new byte[] { 0xd4, 0x8c };
        public static readonly byte[] TgSetData = new byte[] { 0xd4, 0x8e };

        public DirectCommand(byte[] cmd)
            : base((byte)Cla.ReservedForPts, 0, 0, 0, (byte)cmd.Length, cmd)
        {
        }
    }
}

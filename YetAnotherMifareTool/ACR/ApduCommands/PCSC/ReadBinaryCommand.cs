namespace YetAnotherMifareTool.ACR
{
    /// <summary>
    /// PCSC ReadBinary command
    /// </summary>
    public class ReadBinaryCommand : ApduCommand
    {
        public ReadBinaryCommand(ushort address, byte? expectedReturnBytes)
            : base((byte)Cla.ReservedForPts, (byte)Ins.ReadBinary, 0, 0, null, expectedReturnBytes)
        {
            this.Address = address;
        }

        public ushort Address
        {
            set
            {
                base.P1 = (byte)(value >> 8);
                base.P2 = (byte)(value & 0x00FF);
            }
            get { return (ushort)((base.P1 << 8) | base.P2); }
        }
    }
}

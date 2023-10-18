namespace YetAnotherMifareTool.ACR
{
    /// <summary>
    /// PCSC Updatebinary Command
    /// </summary>
    public class UpdateBinaryCommand : ApduCommand
    {
        public UpdateBinaryCommand(ushort address, byte[] dataToWrite)
            : base((byte)Cla.ReservedForPts, (byte)Ins.UpdateBinary, 0, 0, dataToWrite, null)
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

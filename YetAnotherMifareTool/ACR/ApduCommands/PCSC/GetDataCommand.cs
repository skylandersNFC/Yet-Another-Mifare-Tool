namespace YetAnotherMifareTool.ACR
{
    /// <summary>
    /// PCSC GetData command
    /// </summary>
    public class GetDataCommand : ApduCommand
    {
        public enum GetDataDataType : byte
        {
            Uid = 0x00,
            HistoricalBytes = 0x01 // Returned data excludes CRC
        }
        public GetDataDataType Type
        {
            set { base.P1 = (byte)value; }
            get { return (GetDataDataType)base.P1; }
        }
        public GetDataCommand(GetDataDataType type)
            : base((byte)Cla.ReservedForPts, (byte)Ins.GetData, (byte)type, 0, null, 0)
        {
        }
    }
}

namespace YetAnotherMifareTool.ACR
{
    /// <summary>
    /// PCSC GetFirmware command
    /// </summary>
    public class GetFirmwareCommand : ApduCommand
    {
        public GetFirmwareCommand()
            : base((byte)Cla.ReservedForPts, 0, 0x48, 0, null, 0)
        {
        }
    }
}

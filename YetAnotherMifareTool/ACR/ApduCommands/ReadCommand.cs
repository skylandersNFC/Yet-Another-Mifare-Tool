namespace YetAnotherMifareTool.ACR
{
    /// <summary>
    /// Mifare Standard Read command when sent to the card the card is expected to return 16 bytes
    /// </summary>
    public class ReadCommand : ReadBinaryCommand
    {
        public ReadCommand(ushort address)
            : base(address, 16)
        {
        }
    }
}

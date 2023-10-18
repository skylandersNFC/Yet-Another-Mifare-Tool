namespace YetAnotherMifareTool.ACR
{
    /// <summary>
    /// PCSC GetHistoricalBytes command
    /// </summary>
    public class GetHistoricalBytesCommand : GetDataCommand
    {
        public GetHistoricalBytesCommand()
            : base(GetDataCommand.GetDataDataType.HistoricalBytes)
        {
        }
    }
}

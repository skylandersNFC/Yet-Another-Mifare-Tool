namespace YetAnotherMifareTool.ACR
{
    /// <summary>
    /// PCSC GetUid command
    /// </summary>
    public class GetUidCommand : GetDataCommand
    {
        public GetUidCommand()
            : base(GetDataCommand.GetDataDataType.Uid)
        {
        }
    }
}

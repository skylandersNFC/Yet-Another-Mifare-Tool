namespace YetAnotherMifareTool.ACR
{
    internal class NoResponse : ApduResponse
    {
        public override bool Succeeded { get { return false; } }
    }
}

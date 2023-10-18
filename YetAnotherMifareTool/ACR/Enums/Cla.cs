namespace YetAnotherMifareTool.ACR
{
    /// <summary>
    /// Enumeration of possible ISO 7816 Command 
    /// </summary>
    public enum Cla : byte
    {
        CompliantCmd0x = 0x00,
        AppCompliantCmdAx = 0xA0,
        ProprietaryCla8x = 0x80,
        ProprietaryCla9x = 0x90,
        ReservedForPts = 0xFF,           // Protocol Type Selelction
    }
}

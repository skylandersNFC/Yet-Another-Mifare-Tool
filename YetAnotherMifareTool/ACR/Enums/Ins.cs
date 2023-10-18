namespace YetAnotherMifareTool.ACR
{
    /// <summary>
    /// Enumeration of possible instructions 
    /// </summary>
    public enum Ins : byte
    {
        EraseBinary = 0x0E,
        Verify = 0x20,
        ManageChannel = 0x70,
        ExternalAuthenticate = 0x82,
        GetChallenge = 0x84,
        InternalAuthenticate = 0x88,
        SelectFile = 0xA4,
        ReadBinary = 0xB0,
        ReadRecords = 0xB2,
        GetResponse = 0xC0,
        Envelope = 0xC2,
        GetData = 0xCA,
        WriteBinary = 0xD0,
        WriteRecord = 0xD2,
        UpdateBinary = 0xD6,
        PutData = 0xDA,
        UpdateData = 0xDC,
        AppendRecord = 0xE2,
        LoadKeys = 0x82,
        GeneralAuthenticate = 0x86,
    }
}

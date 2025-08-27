namespace LibnfcSharp.Mifare.Enums
{
    internal enum MifareResponseType
    {
        ACK = 0x0A,     // 1010 - ACK
        NACK_NA = 0x04, // 0100 - NACK, not allowed (command not allowed)
        NACK_TR = 0x05  // 0101 - NACK, transmission error
    }
}

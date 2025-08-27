namespace LibnfcSharp.Mifare.Enums
{
    internal enum MifareCommandType
    {
        AUTH_A = 0x60,
        AUTH_B = 0x61,
        READ = 0x30,
        WRITE = 0xA0,
        TRANSFER = 0xB0,
        DECREMENT = 0xC0,
        INCREMENT = 0xC1,
        STORE = 0xC2
    }
}

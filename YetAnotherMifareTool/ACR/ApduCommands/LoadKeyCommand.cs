namespace YetAnotherMifareTool.ACR
{
    /// <summary>
    /// Mifare Standard Load Keys commad which stores the supplied key into the specified numbered key slot
    /// for subsequent use by the General Authenticate command.
    /// </summary>
    public class LoadKeyCommand : LoadKeysCommand
    {
        public LoadKeyCommand(byte[] mifareKey, byte keySlotNumber)
            : base(LoadKeysKeyType.CardKey, null, LoadKeysTransmissionType.Plain, GetLoadKeysType(), keySlotNumber, mifareKey)
        {
        }

        private static LoadKeysStorageType GetLoadKeysType()
        {
            return LoadKeysStorageType.Volatile;
        }
    }
}

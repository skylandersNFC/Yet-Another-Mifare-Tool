namespace YetAnotherMifareTool.ACR
{
    /// <summary>
    /// PCSC LoadKeys command
    /// </summary>
    public class LoadKeysCommand : ApduCommand
    {
        public enum LoadKeysKeyType : byte
        {
            CardKey = 0x00,
            ReaderKey = 0x80,

            Mask = 0x80,
        }
        public enum LoadKeysTransmissionType : byte
        {
            Plain = 0x00,
            Secured = 0x40,

            Mask = 0x40,
        }
        public enum LoadKeysStorageType : byte
        {
            Volatile = 0x00,
            NonVolatile = 0x20,

            Mask = 0x20,
        }
        public LoadKeysKeyType KeyType
        {
            set { base.P1 = (byte)((base.P1 & ~(byte)LoadKeysKeyType.Mask) | (byte)(value & LoadKeysKeyType.Mask)); }
            get { return (LoadKeysKeyType)(base.P1 & (byte)LoadKeysKeyType.Mask); }
        }
        public LoadKeysTransmissionType TransmissionType
        {
            set { base.P1 = (byte)((base.P1 & ~(byte)LoadKeysTransmissionType.Mask) | (byte)(value & LoadKeysTransmissionType.Mask)); }
            get { return (LoadKeysTransmissionType)(base.P1 & (byte)LoadKeysTransmissionType.Mask); }
        }
        public LoadKeysStorageType StorageType
        {
            set { base.P1 = (byte)((base.P1 & ~(byte)LoadKeysStorageType.Mask) | (byte)(value & LoadKeysStorageType.Mask)); }
            get { return (LoadKeysStorageType)(base.P1 & (byte)LoadKeysStorageType.Mask); }
        }
        public byte ReaderKeyNumber
        {
            set { base.P1 = (byte)((base.P1 & 0xF0) | (byte)(value & 0x0F)); }
            get { return (byte)(base.P1 & 0x0F); }
        }
        public byte KeyNumber
        {
            set { base.P2 = value; }
            get { return base.P2; }
        }
        public byte[] KeyData
        {
            set { base.CommandData = value; }
            get { return base.CommandData; }
        }
        public LoadKeysCommand(LoadKeysKeyType keyType, byte? readerKeyNumber, LoadKeysTransmissionType transmissionType, LoadKeysStorageType storageType, byte keyNumber, byte[] keyData)
            : base((byte)Cla.ReservedForPts, (byte)Ins.LoadKeys, (byte)((byte)keyType | (byte)transmissionType | (byte)storageType | (readerKeyNumber ?? 0)), keyNumber, keyData, null)
        {
        }
    }
}

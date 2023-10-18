namespace YetAnotherMifareTool.ACR
{
    /// <summary>
    /// PCSC GeneralAuthenticate command
    /// </summary>
    public class GeneralAuthenticateCommand : ApduCommand
    {
        public enum GeneralAuthenticateKeyType : byte
        {
            MifareKeyA = 0x60,
            PicoTagPassKeyB = 0x61
        }
        public enum GeneralAuthenticateVersionNumber : byte
        {
            VersionOne = 0x01
        }
        public GeneralAuthenticateVersionNumber VersionNumber
        {
            set { base.CommandData[0] = (byte)value; }
            get { return (GeneralAuthenticateVersionNumber)base.CommandData[0]; }
        }
        public ushort Address
        {
            set
            {
                base.CommandData[1] = (byte)(value >> 8);
                base.CommandData[2] = (byte)(value & 0x00FF);
            }
            get { return (ushort)((base.CommandData[1] << 8) | base.CommandData[2]); }
        }
        public byte KeyType
        {
            set { base.CommandData[3] = value; }
            get { return base.CommandData[3]; }
        }
        public byte KeyNumber
        {
            set { base.CommandData[4] = value; }
            get { return base.CommandData[4]; }
        }
        public GeneralAuthenticateCommand(GeneralAuthenticateVersionNumber version, ushort address, GeneralAuthenticateKeyType keyType, byte keyNo)
            : base((byte)Cla.ReservedForPts, (byte)Ins.GeneralAuthenticate, 0, 0, new byte[5] { (byte)version, (byte)(address >> 8), (byte)(address & 0x00FF), (byte)keyType, keyNo }, null)
        {
        }
    }
}

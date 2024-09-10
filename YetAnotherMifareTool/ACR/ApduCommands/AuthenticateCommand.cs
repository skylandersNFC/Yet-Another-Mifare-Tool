using System;

namespace YetAnotherMifareTool.ACR
{
    /// <summary>
    /// Mifare Standard GetHistoricalBytes command
    /// </summary>
    public class AuthenticateCommand : GeneralAuthenticateCommand
    {
        public AuthenticateCommand(ushort address, byte keySlotNumber, GeneralAuthenticateKeyType keyType)
            : base(GeneralAuthenticateVersionNumber.VersionOne, address, keyType, keySlotNumber)
        {
            if (keyType != GeneralAuthenticateKeyType.MifareKeyA && keyType != GeneralAuthenticateKeyType.PicoTagPassKeyB)
            {
                throw new Exception("Invalid key type for MIFARE Standard General Authenticate");
            }
        }
    }
}

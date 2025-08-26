using LibnfcSharp.Mifare.Enums;
using System.Linq;

namespace LibnfcSharp.Mifare
{
    public partial class MifareClassic
    {
        public bool ResetCard(byte[] blocksToReset, byte[] blockData)
        {
            _logCallback?.Invoke(LogLevel.Information, "Resetting card...");

            byte lastAuthenticatedSector = 0xFF;

            foreach (var blockToReset in blocksToReset.OrderBy(x => x))
            {
                var currentSector = GetSector(blockToReset);
                if (currentSector != lastAuthenticatedSector)
                {
                    if (MagicCardType == MifareMagicCardType.GEN_1 ||
                        MagicCardType == MifareMagicCardType.GEN_2)
                    {
                        if (Authenticate(currentSector, MifareKeyType.KEY_A, FACTORY_KEY) ||
                            Authenticate(currentSector, MifareKeyType.KEY_A, _keyAProviderCallback?.Invoke(currentSector, Uid)))
                        {
                            _logCallback?.Invoke(LogLevel.Debug, $"Sector {currentSector} authenticated successfully.");
                            lastAuthenticatedSector = currentSector;
                        }
                        else
                        {
                            _logCallback?.Invoke(LogLevel.Error, $"Error: Authenticating sector {currentSector} failed!");
                            return false;
                        }
                    }
                }

                if (WriteBlock(blockToReset, blockData))
                {
                    _logCallback?.Invoke(LogLevel.Debug, $"Block {blockToReset} reseted successfully.");
                }
                else
                {
                    _logCallback?.Invoke(LogLevel.Error, $"Error: Writing block {blockToReset} failed!");
                    return false;
                }
            }

            _logCallback?.Invoke(LogLevel.Information, "Card reseted successfully.");

            return true;
        }
    }
}

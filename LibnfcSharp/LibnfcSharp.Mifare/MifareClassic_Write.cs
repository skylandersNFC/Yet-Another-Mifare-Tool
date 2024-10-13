using LibnfcSharp.Mifare.Enums;
using LibnfcSharp.PInvoke;
using System;

namespace LibnfcSharp.Mifare
{
    public partial class MifareClassic
    {
        private bool WriteManufacturerBlock(byte[] manufacturerBlockData)
        {
            if (MagicCardType == MifareMagicCardType.GEN_1 ||
                MagicCardType == MifareMagicCardType.GEN_2)
            {
                if (!Authenticate(0, MifareKeyType.KEY_A, FACTORY_KEY) &&
                    !Authenticate(0, MifareKeyType.KEY_A, _keyAProviderCallback?.Invoke(0, Uid)))
                {
                    return false;
                }

            }

            return WriteBlock(0, manufacturerBlockData);
        }

        public bool WriteDump(byte[] dumpData)
        {
            _logCallback?.Invoke("Writing dump...");

            bool success = false;

            for (byte sector = 0; sector < SECTOR_COUNT; sector++)
            {
                var buffer = new byte[BLOCK_SIZE * BLOCKS_PER_SECTOR];
                Array.Copy(dumpData, sector * buffer.Length, buffer, 0, buffer.Length);

                success = WriteSector(sector, buffer);
                if (!success)
                {
                    break;
                }
            }

            if (success &&
                (MagicCardType == MifareMagicCardType.GEN_1A ||
                 MagicCardType == MifareMagicCardType.GEN_1B ||
                 MagicCardType == MifareMagicCardType.GEN_2))
            {
                var buffer = new byte[BLOCK_SIZE];
                Array.Copy(dumpData, 0, buffer, 0, buffer.Length);

                success = WriteManufacturerBlock(buffer);
                if (success)
                {
                    _logCallback?.Invoke($"Manufacturer block written successfully.");
                }
                else
                {
                    _logCallback?.Invoke($"Error: Writing manufacturer block failed!");
                }
            }

            if (success)
                _logCallback?.Invoke("Dump written successfully.");

            return success;
        }

        public bool WriteSector(byte sector, byte[] sectorData)
        {
            var hasUnlockedAccessConditions = true;

            if (MagicCardType == MifareMagicCardType.GEN_1 ||
                MagicCardType == MifareMagicCardType.GEN_2)
            {
                if (Authenticate(sector, MifareKeyType.KEY_A, FACTORY_KEY) ||
                    Authenticate(sector, MifareKeyType.KEY_A, _keyAProviderCallback?.Invoke(sector, Uid)))
                {
                    _logCallback?.Invoke($"Sector {sector} authenticated successfully.");
                }
                else
                {
                    _logCallback?.Invoke($"Error: Authenticating sector {sector} failed!");
                    return false;
                }

                hasUnlockedAccessConditions = HasUnlockedAccessConditions(sector, out _, true);
            }

            var skippingSectorZero = false;
            byte startBlock = (byte)(sector == 0 ? 1 : 0);

            for (byte block = startBlock; block < BLOCKS_PER_SECTOR; block++)
            {
                byte[] buffer = new byte[BLOCK_SIZE];
                Array.Copy(sectorData, block * BLOCK_SIZE, buffer, 0, buffer.Length);
                var globalBlock = GetGlobalBlock(sector, block);

                if (WriteBlock(globalBlock, buffer))
                {
                    _logCallback?.Invoke($"Block {globalBlock} written successfully.");
                    skippingSectorZero = false;
                }
                else
                if (sector == 0 && !hasUnlockedAccessConditions)
                {
                    _logCallback?.Invoke($"Skipping block {globalBlock} (Sector 0).");
                    skippingSectorZero = true;
                }
                else
                if (IsTrailerBlock(block) && !hasUnlockedAccessConditions)
                {
                    _logCallback?.Invoke($"Skipping block {globalBlock} (Trailer block).");
                }
                else
                {
                    _logCallback?.Invoke($"Error: Writing Block {globalBlock} failed!");
                    return false;
                }
            }

            if (skippingSectorZero)
            {
                _logCallback?.Invoke($"Sector {sector} skipped.");
            }
            else
            {
                _logCallback?.Invoke($"Sector {sector} written successfully.");
            }

            return true;
        }

        public bool WriteBlock(byte block, byte[] blockData)
        {
            if (blockData == null || blockData.Length != BLOCK_SIZE || block >= BLOCKS_TOTAL_COUNT)
                return false;

            byte[] abtCmd = new byte[18];
            abtCmd[0] = (byte)MifareCommandType.WRITE;
            abtCmd[1] = block;
            Array.Copy(blockData, 0, abtCmd, 2, BLOCK_SIZE);

            if (!TransmitBytes(abtCmd, (uint)abtCmd.Length, out int result))
            {
                if (result == (int)NfcError.NFC_ERFTRANS)
                {
                    // "Invalid received frame", usual means we are
                    // authenticated on a sector but the requested MIFARE cmd (read, write)
                    // is not permitted by current acces bytes;
                    // So there is nothing to do here.
                }
                else
                {
                    Perror("nfc_initiator_transceive_bytes");
                }
                SelectCard();

                return false;
            }

            return true;
        }
    }
}
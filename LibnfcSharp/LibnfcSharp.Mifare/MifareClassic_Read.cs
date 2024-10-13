using LibnfcSharp.Mifare.Enums;
using LibnfcSharp.Mifare.Models;
using LibnfcSharp.PInvoke;
using System;

namespace LibnfcSharp.Mifare
{
    public partial class MifareClassic
    {
        public bool ReadManufacturerInfo(out ManufacturerInfo manufacturerInfo)
        {
            var result = ReadManufacturerBlock(out byte[] manufacturerBlock);
            manufacturerInfo = new ManufacturerInfo(manufacturerBlock);
            return result;
        }

        public bool ReadManufacturerBlock(out byte[] manufacturerBlockData)
        {
            manufacturerBlockData = new byte[16];

            if (MagicCardType == MifareMagicCardType.GEN_1 ||
                MagicCardType == MifareMagicCardType.GEN_2)
            {
                if (!Authenticate(0, MifareKeyType.KEY_A, FACTORY_KEY) &&
                    !Authenticate(0, MifareKeyType.KEY_A, _keyAProviderCallback?.Invoke(0, Uid)))
                {
                    return false;
                }

            }

            return ReadBlock(0, out manufacturerBlockData);
        }

        public bool ReadAccessConditions(byte sector, out byte[] accessConditions, bool skipAuthentication = false)
        {
            accessConditions = new byte[ACS_SIZE];

            var trailerBlock = GetTrailerBlock((byte)(sector * BLOCKS_PER_SECTOR));

            if (MagicCardType == MifareMagicCardType.GEN_1 ||
                MagicCardType == MifareMagicCardType.GEN_2)
            {
                if (!skipAuthentication &&
                    !Authenticate(sector, MifareKeyType.KEY_A, FACTORY_KEY) &&
                    !Authenticate(sector, MifareKeyType.KEY_A, _keyAProviderCallback?.Invoke(0, Uid)))
                {
                    return false;
                }

            }

            if (ReadBlock(trailerBlock, out byte[] blockData))
            {
                Array.Copy(blockData, ACS_OFFSET, accessConditions, 0, accessConditions.Length);

                return true;
            }
            return false;
        }

        public bool ReadCard(out byte[] cardData)
        {
            _logCallback?.Invoke("Reading card...");

            cardData = new byte[BLOCK_SIZE * BLOCKS_TOTAL_COUNT];

            for (byte sector = 0; sector < SECTOR_COUNT; sector++)
            {
                if (ReadSector(sector, out byte[] sectorData))
                {
                    Array.Copy(sectorData, 0, cardData, sector * BLOCK_SIZE * BLOCKS_PER_SECTOR, sectorData.Length);
                }
                else
                {
                    return false;
                }
            }

            _logCallback?.Invoke("Card read successfully.");

            return true;
        }

        public bool ReadSector(byte sector, out byte[] sectorData)
        {
            sectorData = new byte[BLOCK_SIZE * BLOCKS_PER_SECTOR];

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
            }

            for (byte block = 0; block < BLOCKS_PER_SECTOR; block++)
            {
                var globalBlock = GetGlobalBlock(sector, block);

                if (ReadBlock(globalBlock, out byte[] blockData))
                {
                    Array.Copy(blockData, 0, sectorData, block * BLOCK_SIZE, blockData.Length);

                    _logCallback?.Invoke($"Block {globalBlock} read successfully.");
                }
                else
                {
                    _logCallback?.Invoke($"Error: Reading Block {globalBlock} failed!");
                    return false;
                }
            }

            _logCallback?.Invoke($"Sector {sector} read successfully.");

            return true;
        }

        public bool ReadBlock(byte block, out byte[] blockData)
        {
            blockData = new byte[BLOCK_SIZE];

            if (block >= BLOCKS_TOTAL_COUNT)
                return false;

            byte[] abtCmd = new byte[2];
            abtCmd[0] = (byte)MifareCommandType.READ;
            abtCmd[1] = block;

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

            if (result == BLOCK_SIZE)
            {
                Array.Copy(_rxBuffer, 0, blockData, 0, blockData.Length);
            }
            else
            {
                SelectCard();

                return false;
            }

            return true;
        }
    }
}
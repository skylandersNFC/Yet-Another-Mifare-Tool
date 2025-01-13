using LibnfcSharp.Mifare.Enums;
using LibnfcSharp.PInvoke;
using System;
using System.Linq;

namespace LibnfcSharp.Mifare
{
    public partial class MifareClassic
    {
        public const byte UID_SIZE = 4;
        public const byte ACS_SIZE = 4;
        public const byte ACS_OFFSET = 6;
        public const byte KEY_SIZE = 6;
        public const byte BLOCK_SIZE = 16;
        public const byte SECTOR_COUNT = 16;
        public const byte BLOCKS_PER_SECTOR = 4;
        public const byte BLOCKS_TOTAL_COUNT = BLOCKS_PER_SECTOR * SECTOR_COUNT;

        public static readonly byte[] FACTORY_KEY = new byte[KEY_SIZE] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
        public static readonly byte[] DEFAULT_ACS = new byte[ACS_SIZE] { 0x7F, 0x0F, 0x08, 0x69 };
        public static readonly byte[] LOCKED_ACS = new byte[ACS_SIZE] { 0x0F, 0x0F, 0x0F, 0x69 };
        public static readonly byte[] UNLOCKED_ACS = new byte[ACS_SIZE] { 0xFF, 0x07, 0x80, 0x69 };
        public static readonly byte[] EMPTY_BLOCK = new byte[BLOCK_SIZE] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        private byte[] _rxBuffer = new byte[264];
        private NfcDevice _device;
        private NfcTarget _target;
        private bool _enablePerrorLogging = false;
        private Func<byte, byte[], byte[]> _keyAProviderCallback;
        private Action<string> _logCallback;

        private MifareMagicCardType _magicCardType = MifareMagicCardType.GEN_1;

        public MifareMagicCardType MagicCardType { get { return _magicCardType; } }
        public byte[] Uid { get { return _target.TargetInfo.Iso14443aInfo.abtUid.Take((int)_target.TargetInfo.Iso14443aInfo.szUidLen).ToArray(); } }
        public byte Sak { get { return _target.TargetInfo.Iso14443aInfo.btSak; } }
        public byte[] Atqa { get { return _target.TargetInfo.Iso14443aInfo.abtAtqa; } }
        public byte[] Ats { get { return _target.TargetInfo.Iso14443aInfo.abtAts.Take((int)_target.TargetInfo.Iso14443aInfo.szAtsLen).ToArray(); } }

        public MifareClassic(NfcDevice device, bool enablePerrorLogging = false)
        {
            _target = new NfcTarget();
            _device = device;
            _enablePerrorLogging = enablePerrorLogging;
        }

        public void RegisterKeyAProviderCallback(Func<byte, byte[], byte[]> keyAProviderCallback) =>
            _keyAProviderCallback = keyAProviderCallback;

        public void RegisterLogCallback(Action<string> logCallback) =>
            _logCallback = logCallback;

        public void InitialDevice()
        {
            // Initialise NFC device as "initiator"
            if (!_device.InitiatorInit())
            {
                Perror("nfc_initiator_init");
            }

            // Let the reader only try once to find a tag
            if (!_device.DeviceSetPropertyBool(NfcProperty.InfiniteSelect, false))
            {
                Perror("nfc_device_set_property_bool");
            }

            // Disable ISO14443-4 switching in order to read devices that emulate Mifare Classic with ISO14443-4 compliance.
            if (!_device.DeviceSetPropertyBool(NfcProperty.AutoIso14443_4, false))
            {
                Perror("nfc_device_set_property_bool");
            }
        }

        public bool SelectCard()
        {
            var modulation = new NfcModulation
            {
                ModulationType = NfcModulationType.NMT_ISO14443A,
                BaudRate = NfcBaudRate.NBR_106
            };

            return _device.InitiatorSelectPassiveTarget(modulation, null, 0, ref _target) > 0;
        }

        public void WaitForCard()
        {
            while (!SelectCard());
        }

        public void IdentifyMagicCardType()
        {
            _magicCardType = MifareMagicCardType.GEN_1;

            if (!UnlockCard(out _magicCardType))
            {
                // Reselect card
                InitialDevice();
                WaitForCard();

                _magicCardType = IsMagicGen2()
                    ? MifareMagicCardType.GEN_2
                    : MifareMagicCardType.GEN_1;
            }
        }

        public bool HasUnlockedAccessConditions(byte sector, out byte[] accessConditions, bool skipAuthentication = false)
        {
            return ReadAccessConditions(sector, out accessConditions, skipAuthentication) &&
                accessConditions.SequenceEqual(UNLOCKED_ACS);
        }

        public bool UnlockCard(out MifareMagicCardType magicCardType)
        {
            magicCardType = MifareMagicCardType.GEN_1;

            byte[] abtHalt = { 0x50, 0x00, 0x00, 0x00 };

            // special unlock command
            byte[] abtUnlock1 = { 0x40 };
            byte[] abtUnlock2 = { 0x43 };

            // Configure the CRC
            if (!_device.DeviceSetPropertyBool(NfcProperty.HandleCrc, false))
            {
                Perror("nfc_device_set_property_bool");
                return false;
            }

            // Use raw send/receive methods
            if (!_device.DeviceSetPropertyBool(NfcProperty.EasyFraming, false))
            {
                Perror("nfc_device_set_property_bool");
                return false;
            }

            _device.Iso14443aCrcAppend(abtHalt, 2);
            TransmitBytes(abtHalt, 4);

            // now send unlock1 => Gen1B
            if (TransmitBits(abtUnlock1, 7) && _rxBuffer[0] == (byte)MifareResponseType.ACK) // transmit_bits(abtUnlock1, 7)
            {
                magicCardType = MifareMagicCardType.GEN_1B;

                // then send unlock2 => Gen1A
                if (TransmitBytes(abtUnlock2, 1) && _rxBuffer[0] == (byte)MifareResponseType.ACK) // transmit_bytes(abtUnlock2, 1)
                {
                    magicCardType = MifareMagicCardType.GEN_1A;
                }
            }

            // reset reader
            // Configure the CRC
            if (!_device.DeviceSetPropertyBool(NfcProperty.HandleCrc, true))
            {
                Perror("nfc_device_set_property_bool");
                return false;
            }

            // Switch off raw send/receive methods
            if (!_device.DeviceSetPropertyBool(NfcProperty.EasyFraming, true))
            {
                Perror("nfc_device_set_property_bool");
                return false;
            }

            return magicCardType != MifareMagicCardType.GEN_1;
        }

        public bool Authenticate(byte sector, MifareKeyType keyType, byte[] key)
        {
            if (key == null || key.Length != KEY_SIZE || sector >= SECTOR_COUNT)
                return false;

            byte[] abtCmd = new byte[12];
            abtCmd[0] = (byte)(keyType == MifareKeyType.KEY_A ? MifareCommandType.AUTH_A : MifareCommandType.AUTH_B);
            abtCmd[1] = (byte)(sector * BLOCKS_PER_SECTOR);
            Array.Copy(key, 0, abtCmd, 2, KEY_SIZE);
            Array.Copy(Uid, 0, abtCmd, 8, UID_SIZE);

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
                // if auth failed mifare card needs to be reselected
                SelectCard();

                return false;
            }

            return true;
        }

        private bool IsMagicGen2()
        {
            var gen2Ats = new byte[9] { 0x09, 0x78, 0x00, 0x91, 0x02, 0xDA, 0xBC, 0x19, 0x10 };

            if (_target.TargetInfo.Iso14443aInfo.szAtsLen > 0 && _target.TargetInfo.Iso14443aInfo.abtAts.Take(gen2Ats.Length).SequenceEqual(gen2Ats))
            {
                return true;
            }
            else
            {
                if (Authenticate(0, MifareKeyType.KEY_A, FACTORY_KEY) ||
                    Authenticate(0, MifareKeyType.KEY_A, _keyAProviderCallback?.Invoke(0, Uid)))
                {
                    if (ReadBlock(0, out byte[] blockData))
                    {
                        if (WriteBlock(0, blockData))
                        {
                            // abort to ensure nothing is written to block 0
                            // AbortCommand runs after already written :(
                            //_device.AbortCommand();

                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool IsFirstBlock(byte block) =>
            block % 4 == 0;

        public static bool IsTrailerBlock(byte block) =>
            (block + 1) % 4 == 0;

        public static byte GetTrailerBlock(byte block) =>
            (byte)(4 * ((block / 4) + 1) - 1);

        public static byte GetGlobalBlock(byte sector, byte blockOfSector) =>
            (byte)(blockOfSector + (sector * BLOCKS_PER_SECTOR));

        public static byte GetSector(byte globalBlock) =>
            (byte)(globalBlock / 4);

        private void Perror(string source)
        {
            if (_enablePerrorLogging)
            {
                _device.Perror(source);
            }
        }

        private bool TransmitBits(byte[] pbtTx, uint szTxBits) =>
            _device.InitiatorTransceiveBits(pbtTx, szTxBits, null, _rxBuffer, (uint)_rxBuffer.Length, null) >= 0;

        private bool TransmitBytes(byte[] pbtTx, uint szTx) =>
            TransmitBytes(pbtTx, szTx, out _);

        private bool TransmitBytes(byte[] pbtTx, uint szTx, out int result) =>
            (result = _device.InitiatorTransceiveBytes(pbtTx, szTx, _rxBuffer, (uint)_rxBuffer.Length, 0)) >= 0;
    }
}
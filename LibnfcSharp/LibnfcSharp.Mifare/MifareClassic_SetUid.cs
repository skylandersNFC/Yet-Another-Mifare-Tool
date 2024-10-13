using LibnfcSharp.PInvoke;
using System;

namespace LibnfcSharp.Mifare
{
    public partial class MifareClassic
    {
        public bool SetUid(byte[] uidOrBlock0 = null, bool format = false)
        {
            byte[] abtRawUid = new byte[12];
            byte[] abtAtqa = new byte[2];
            byte abtSak;

            int szCL = 1; //Always start with Cascade Level 1 (CL1)

            bool iso_ats_supported = false;

            // ISO14443A Anti-Collision Commands
            byte[] abtReqa = { 0x26 };
            byte[] abtSelectAll = { 0x93, 0x20 };
            byte[] abtSelectTag = { 0x93, 0x70, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            byte[] abtRats = { 0xe0, 0x50, 0x00, 0x00 };
            byte[] abtHalt = { 0x50, 0x00, 0x00, 0x00 };

            const byte CASCADE_BIT = 0x04;
            const byte SAK_FLAG_ATS_SUPPORTED = 0x20;

            // special unlock command
            byte[] abtUnlock1 = { 0x40 };
            byte[] abtUnlock2 = { 0x43 };
            byte[] abtWipe = { 0x41 };
            byte[] abtWrite = { 0xa0, 0x00, 0x5f, 0xb1 };
            byte[] abtData = { 0x01, 0x23, 0x45, 0x67, 0x00, 0x08, 0x04, 0x00, 0x46, 0x59, 0x25, 0x58, 0x49, 0x10, 0x23, 0x02, 0x23, 0xeb };
            byte[] abtBlank = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x07, 0x80, 0x69, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x36, 0xCC };


            if (uidOrBlock0 == null)
            {
                // use default uid
            }
            else
            if (uidOrBlock0.Length == 4 || uidOrBlock0.Length == 16)
            {
                Array.Copy(uidOrBlock0, 0, abtData, 0, uidOrBlock0.Length);
                abtData[4] = (byte)(abtData[0] ^ abtData[1] ^ abtData[2] ^ abtData[3]);
                _device.Iso14443aCrcAppend(abtData, 16);
            }
            else
            {
                _logCallback?.Invoke("Error: Wrong length of UID (4 byte) / Block 0 (16 byte)");
                return false;
            }

            // Initialise NFC device as "initiator"
            if (!_device.InitiatorInit())
            {
                Perror("nfc_initiator_init");
            }

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

            // Disable 14443-4 autoswitching
            if (!_device.DeviceSetPropertyBool(NfcProperty.AutoIso14443_4, false))
            {
                Perror("nfc_device_set_property_bool");
                return false;
            }

            // Send the 7 bits request command specified in ISO 14443A (0x26)
            if (!TransmitBits(abtReqa, 7))
            {
                _logCallback?.Invoke("Error: No tag available");

                return false;
            }

            Array.Copy(_rxBuffer, abtAtqa, 2);

            // Anti-collision
            TransmitBytes(abtSelectAll, 2);

            // Check answer
            if ((_rxBuffer[0] ^ _rxBuffer[1] ^ _rxBuffer[2] ^ _rxBuffer[3] ^ _rxBuffer[4]) != 0)
            {
                _logCallback?.Invoke("WARNING: BCC check failed!");
            }

            // Save the UID CL1
            Array.Copy(_rxBuffer, 0, abtRawUid, 0, 4);


            //Prepare and send CL1 Select-Command
            Array.Copy(_rxBuffer, 0, abtSelectTag, 2, 5);
            _device.Iso14443aCrcAppend(abtSelectTag, 7);
            TransmitBytes(abtSelectTag, 9);
            abtSak = _rxBuffer[0];

            // Test if we are dealing with a CL2
            if ((abtSak & CASCADE_BIT) != 0)
            {
                szCL = 2;//or more

                // Check answer
                if (abtRawUid[0] != 0x88)
                {
                    _logCallback?.Invoke("WARNING: Cascade bit set but CT != 0x88!");
                }
            }

            if (szCL == 2)
            {
                // We have to do the anti-collision for cascade level 2

                // Prepare CL2 commands
                abtSelectAll[0] = 0x95;

                // Anti-collision
                TransmitBytes(abtSelectAll, 2);

                // Check answer
                if ((_rxBuffer[0] ^ _rxBuffer[1] ^ _rxBuffer[2] ^ _rxBuffer[3] ^ _rxBuffer[4]) != 0)
                {
                    _logCallback?.Invoke("WARNING: BCC check failed!\n");
                }


                // Save UID CL2
                Array.Copy(_rxBuffer, 0, abtRawUid, 4, 4);

                // Selection
                abtSelectTag[0] = 0x95;
                Array.Copy(_rxBuffer, 0, abtSelectTag, 2, 5);
                _device.Iso14443aCrcAppend(abtSelectTag, 7);
                TransmitBytes(abtSelectTag, 9);
                abtSak = _rxBuffer[0];

                // Test if we are dealing with a CL3
                if ((abtSak & CASCADE_BIT) != 0)
                {
                    szCL = 3;

                    // Check answer
                    if (abtRawUid[0] != 0x88)
                    {
                        _logCallback?.Invoke("WARNING: Cascade bit set but CT != 0x88!");
                    }
                }

                if (szCL == 3)
                {
                    // We have to do the anti-collision for cascade level 3

                    // Prepare and send CL3 AC-Command
                    abtSelectAll[0] = 0x97;
                    TransmitBytes(abtSelectAll, 2);

                    // Check answer
                    if ((_rxBuffer[0] ^ _rxBuffer[1] ^ _rxBuffer[2] ^ _rxBuffer[3] ^ _rxBuffer[4]) != 0)
                    {
                        _logCallback?.Invoke("WARNING: BCC check failed!");
                    }

                    // Save UID CL3
                    Array.Copy(_rxBuffer, 0, abtRawUid, 8, 4);

                    // Prepare and send final Select-Command
                    abtSelectTag[0] = 0x97;
                    Array.Copy(_rxBuffer, 0, abtSelectTag, 2, 5);
                    _device.Iso14443aCrcAppend(abtSelectTag, 7);
                    TransmitBytes(abtSelectTag, 9);
                    abtSak = _rxBuffer[0];
                }
            }

            // Request ATS, this only applies to tags that support ISO 14443A-4
            if ((_rxBuffer[0] & SAK_FLAG_ATS_SUPPORTED) != 0)
            {
                iso_ats_supported = true;
            }

            _logCallback?.Invoke("\nFound tag with\n UID: ");
            switch (szCL)
            {
                case 1:
                    _logCallback?.Invoke($"{abtRawUid[0]:X2}{abtRawUid[1]:X2}{abtRawUid[2]:X2}{abtRawUid[3]:X2}");
                    break;

                case 2:
                    _logCallback?.Invoke($"{abtRawUid[1]:X2}{abtRawUid[2]:X2}{abtRawUid[3]:X2}{abtRawUid[4]:X2}{abtRawUid[5]:X2}{abtRawUid[6]:X2}{abtRawUid[7]:X2}");
                    break;

                case 3:
                    _logCallback?.Invoke($"{abtRawUid[1]:X2}{abtRawUid[2]:X2}{abtRawUid[3]:X2}{abtRawUid[5]:X2}{abtRawUid[6]:X2}{abtRawUid[7]:X2}{abtRawUid[8]:X2}{abtRawUid[9]:X2}{abtRawUid[10]:X2}{abtRawUid[11]:X2}");
                    break;
            }

            _logCallback?.Invoke($"ATQA: {abtAtqa[1]:X2}{abtAtqa[0]:X2}");
            _logCallback?.Invoke($"SAK: {abtSak:X2}");

            // now reset UID
            _device.Iso14443aCrcAppend(abtHalt, 2);
            TransmitBytes(abtHalt, 4);

            if (!TransmitBits(abtUnlock1, 7))
            {
                _logCallback?.Invoke("Warning: Unlock command [1/2]: failed / not acknowledged.");
            }
            else
            {
                if (format)
                {
                    TransmitBytes(abtWipe, 1);
                    TransmitBytes(abtHalt, 4);
                    TransmitBits(abtUnlock1, 7);
                }

                if (TransmitBytes(abtUnlock2, 1))
                {
                    _logCallback?.Invoke("Card unlocked");
                }
                else
                {
                    _logCallback?.Invoke("Warning: Unlock command [2/2]: failed / not acknowledged.");
                }
            }

            TransmitBytes(abtWrite, 4);
            TransmitBytes(abtData, 18);

            if (format)
            {
                for (byte i = 3; i < 64; i += 4)
                {
                    abtWrite[1] = i;
                    _device.Iso14443aCrcAppend(abtWrite, 2);
                    TransmitBytes(abtWrite, 4);
                    TransmitBytes(abtBlank, 18);
                }
            }

            return true;
        }
    }
}

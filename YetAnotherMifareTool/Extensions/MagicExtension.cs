using LibnfcSharp.Mifare;
using System;
using YetAnotherMifareTool.Utils;

namespace YetAnotherMifareTool.Extensions
{
    internal static class MagicExtension
    {
        private static readonly byte[] RESET_BLOCK_IDXS = new byte[42] {
            0x05, 0x06, 0x08, 0x09, 0x0A, 0x0C,
            0x0D, 0x0E, 0x10, 0x11, 0x12, 0x14,
            0x15, 0x16, 0x18, 0x19, 0x1A, 0x1C,
            0x1D, 0x1E, 0x20, 0x21, 0x24, 0x25,
            0x26, 0x28, 0x29, 0x2A, 0x2C, 0x2D,
            0x2E, 0x30, 0x31, 0x32, 0x34, 0x35,
            0x36, 0x38, 0x39, 0x3A, 0x3C, 0x3D
        };

        public static byte[] RecalculateKeys(this byte[] dumpData, byte[] uid)
        {
            byte sector = 0;
            for (int i = 48; i < dumpData.Length; i += 64)
            {
                byte[] key = Crypto.CalculateKeyA(sector, uid);
                Array.Copy(key, 0, dumpData, i, key.Length);
                sector++;
            }
            return dumpData;
        }

        public static byte[] UnlockAccessConditions(this byte[] dumpData)
        {
            byte sector = 0;
            for (int i = 48; i < dumpData.Length; i += 64)
            {
                if (sector == 0)
                {
                    Array.Copy(MifareClassic.UNLOCKED_ACS, 0, dumpData, i + 6, MifareClassic.UNLOCKED_ACS.Length);
                }
                else
                {
                    Array.Copy(MifareClassic.DEFAULT_ACS, 0, dumpData, i + 6, MifareClassic.DEFAULT_ACS.Length);
                }
                sector++;
            }
            return dumpData;
        }

        public static byte[] Reset(this byte[] dumpData)
        {
            foreach (byte blockIndex in RESET_BLOCK_IDXS)
            {
                Array.Copy(MifareClassic.EMPTY_BLOCK, 0, dumpData, blockIndex * MifareClassic.BLOCK_SIZE, MifareClassic.EMPTY_BLOCK.Length);
            }
            return dumpData;
        }
    }
}

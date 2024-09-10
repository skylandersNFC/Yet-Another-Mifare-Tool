using System;

namespace YetAnotherMifareTool.Utils
{
    internal class Magic
    {
        public static byte[] DoTheMagic(byte[] uid, byte sector)
        {
            byte[] key = Constants.SECTOR_ZERO_KEY;
            if (sector > 0)
            {
                key = Precalc.GetOne(uid, sector);
                key = CRC64.Get(key);
            }
            return key;
        }

        public static byte[][] CalculateKeys(byte[] uid)
        {
            var keys = new byte[16][];
            for (int sector = 0; sector < keys.Length; sector++)
            {
                keys[sector] = DoTheMagic(uid, (byte)sector);
            }
            return keys;
        }

        public static byte[] AddRecalculatedKeys(byte[] data)
        {
            byte[] uid = new byte[4];
            Buffer.BlockCopy(data, 0, uid, 0, uid.Length);

            if (data.Length == 4) // only uid in data -> init new data
            {
                data = new byte[1024];
                Buffer.BlockCopy(uid, 0, data, 0, uid.Length);
            }

            byte sector = 0;
            for (int i = 48; i < data.Length; i += 64)
            {
                byte[] key = DoTheMagic(uid, sector);
                Buffer.BlockCopy(key, 0, data, i, key.Length);
                sector++;
            }
            return data;
        }

        public static byte[] UnlockedAccessConditions(byte[] data)
        {
            byte sector = 0;
            for (int i = 48; i < data.Length; i += 64)
            {
                Buffer.BlockCopy(Constants.UNLOCKED_AC[sector], 0, data, i + 6, Constants.UNLOCKED_AC[sector].Length);
                sector++;
            }
            return data;
        }

        public static byte[] Reset(byte[] data)
        {
            foreach (byte blockIndex in Constants.RESET_BLOCK_IDXS)
            {
                byte[] decBlock = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                Buffer.BlockCopy(decBlock, 0, data, blockIndex * Constants.BLOCK_SIZE, decBlock.Length);
            }
            return data;
        }
    }
}

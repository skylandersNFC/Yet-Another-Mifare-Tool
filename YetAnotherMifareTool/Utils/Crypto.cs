namespace YetAnotherMifareTool.Utils
{
    internal class Crypto
    {
        public static ushort ComputeCRC16(byte[] data)
        {
            ushort crc = 0xFFFF;
            for (int i = 0; i < data.Length; i++)
            {
                crc ^= (ushort)(data[i] << 8);
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x8000) > 0)
                        crc = (ushort)((crc << 1) ^ 0x1021);
                    else
                        crc <<= 1;
                }
            }
            return crc;
        }

        public static ulong ComputeCRC48(byte[] data)
        {
            const ulong polynomial = 0x42f0e1eba9ea3693;
            const ulong initialRegisterValue = 2 * 2 * 3 * 1103 * 12868356821;

            ulong crc = initialRegisterValue;
            for (uint i = 0; i < data.Length; i++)
            {
                crc ^= (ulong)data[i] << 40;
                for (byte j = 0; j < 8; j++)
                {
                    if ((crc & 0x800000000000) > 0)
                    {
                        crc = (crc << 1) ^ polynomial;
                    }
                    else
                    {
                        crc <<= 1;
                    }
                    crc &= 0x0000FFFFFFFFFFFF;
                }
            }
            return crc;
        }

        public static byte[] CalculateKeyA(byte sector, byte[] uid)
        {
            if (sector == 0)
            {
                return BitConverter.GetBytes((ulong)73 * 2017 * 560381651).Reverse().Skip(2).ToArray();
            }

            if (uid.Length != 4)
            {
                return new byte[6];
            }

            byte[] data = new byte[5] { uid[0], uid[1], uid[2], uid[3], sector };
            ulong bigEndianCRC = ComputeCRC48(data);
            ulong littleEndianCRC = 0;

            for (byte i = 0; i < 6; i++)
            {
                ulong bte = (bigEndianCRC >> (i * 8)) & 0xFF;
                littleEndianCRC += bte << ((5 - i) * 8);
            }
            return BitConverter.GetBytes(littleEndianCRC).Reverse().Skip(2).ToArray();
        }
    }
}

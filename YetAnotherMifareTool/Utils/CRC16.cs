
namespace YetAnotherMifareTool.Utils
{
    internal class CRC16
	{
		private static ushort UpdateCcittCrc16(uint crc16, byte data)
		{
			uint num = (uint)((ushort)(data << 8));
			for (int i = 0; i < 8; i++)
			{
				uint num2;
				if ((crc16 ^ num) > 32767u)
				{
					num2 = 1u;
				}
				else
				{
					num2 = 0u;
				}
				crc16 = (uint)((ushort)(crc16 << 1 ^ num2 * 4129u));
				num = (uint)((ushort)(num << 1));
			}
			return (ushort)crc16;
		}
		private static ushort ComputeCcittCrc16(byte[] data, uint bytes)
		{
			ushort num = 65535;
			int num2 = 0;
			while ((long)num2 < (long)((ulong)bytes))
			{
				num = CRC16.UpdateCcittCrc16((uint)num, data[num2]);
				num2++;
			}
			return num;
		}
		public static ushort Get(byte[] data, uint bytes)
		{
			return CRC16.ComputeCcittCrc16(data, bytes);
		}
	}
}

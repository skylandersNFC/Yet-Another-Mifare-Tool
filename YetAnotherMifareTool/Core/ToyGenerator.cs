using YetAnotherMifareTool.Utils;

namespace YetAnotherMifareTool.Core
{
    internal class ToyGenerator
    {
        private static byte[] GetChecksumType0(byte[] data)
        {
	        ushort num = CRC16.Get(data, 30u);
	        return new byte[]
	        {
		        (byte)(num & 255),
		        (byte)(num >> 8 & 255)
	        };
        }

        public static byte[] Generate(byte[] block0, uint id, uint idExt)
        {
            byte[] data = new byte[1024];

            Buffer.BlockCopy(block0, 0, data, 0, block0.Length);
             
	        byte[] data_id = BitConverter.GetBytes(id);
            byte[] data_idExt = BitConverter.GetBytes(idExt);
	        data[17] = data_id[1];
	        data[16] = data_id[0];
	        data[29] = data_idExt[1];
	        data[28] = data_idExt[0];
	        byte[] checksumType = GetChecksumType0(data);
	        data[30] = checksumType[0];
	        data[31] = checksumType[1];

            data = data
                .WithCalculatingKeys()
                .WithUnlockedAccessConditions();

            return data;
        }
    }
}

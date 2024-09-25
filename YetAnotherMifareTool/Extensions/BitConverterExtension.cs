using System;

namespace YetAnotherMifareTool.Extensions
{
    internal static class BitConverterExtension
    {
        public static ushort ToUInt16(this byte[] bytes) =>
            BitConverter.ToUInt16(bytes, 0);

        public static byte[] ToBytes(this ushort value) =>
            BitConverter.GetBytes(value);
    }
}

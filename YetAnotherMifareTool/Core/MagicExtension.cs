using YetAnotherMifareTool.Utils;

namespace YetAnotherMifareTool.Core
{
    public static class MagicExtension
    {
        public static byte[] WithResetting(this byte[] data, bool useResetting = true)
        {
            return useResetting
                ? Magic.Reset(data)
                : data;
        }

        public static byte[] WithCalculatingKeys(this byte[] data)
        {
            return Magic.CalculateKeys(data);
        }

        public static byte[] WithUnlockedAccessConditions(this byte[] data)
        {
            return Magic.UnlockedAccessConditions(data);
        }
    }
}

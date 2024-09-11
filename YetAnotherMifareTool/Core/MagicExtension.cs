using YetAnotherMifareTool.Utils;

namespace YetAnotherMifareTool.Core
{
    internal static class MagicExtension
    {
        public static byte[] WithResetting(this byte[] data, bool useResetting = true)
        {
            return useResetting
                ? Magic.Reset(data)
                : data;
        }

        public static byte[] WithRecalculatedKeys(this byte[] data)
        {
            return Magic.AddRecalculatedKeys(data);
        }

        public static byte[] WithUnlockedAccessConditions(this byte[] data)
        {
            return Magic.UnlockedAccessConditions(data);
        }
    }
}

using System.ComponentModel;

namespace LibnfcSharp.Mifare.Extensions
{
    public static class EnumExtension
    {
        public static string ToDescription<T>(this T enumSource)
        {
            var fi = enumSource.GetType().GetField(enumSource.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes != null && attributes.Length > 0
                ? attributes[0].Description
                : enumSource.ToString();
        }
    }
}

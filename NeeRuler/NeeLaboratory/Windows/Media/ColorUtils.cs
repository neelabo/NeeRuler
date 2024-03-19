using System.Windows.Media;

namespace NeeLaboratory.Windows.Media
{
    public static class ColorUtils
    {
        private static readonly ColorConverter _colorConverter =  new();

        public static Color FromCode(uint value)
        {
            var a = (byte)((value & 0xFF000000) >> 24);
            var r = (byte)((value & 0xFF0000) >> 16);
            var g = (byte)((value & 0xFF00) >> 8);
            var b = (byte)(value & 0xFF);
            return Color.FromArgb(a, r, g, b);
        }

        public static Color? FromString(string? s)
        {
            if (s is null) return null;

            return (Color)_colorConverter.ConvertFrom(s);
        }
    }
}
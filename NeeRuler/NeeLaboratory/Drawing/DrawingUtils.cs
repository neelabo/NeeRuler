using System.Drawing;
using System.Windows;

namespace NeeLaboratory.Drawing
{
    public static class DrawingUtils
    {
        public static Bitmap CaptureScreen(Rect rect)
        {
            var bitmap = new Bitmap((int)rect.Width, (int)rect.Height);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen((int)rect.X, (int)rect.Y, 0, 0, bitmap.Size);
            }
            return bitmap;
        }
    }
}

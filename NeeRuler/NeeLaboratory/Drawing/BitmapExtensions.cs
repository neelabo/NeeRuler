using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace NeeLaboratory.Drawing
{
    public static class BitmapExtensions
    {
        private static class NativeMethods
        {
            [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool DeleteObject([In] IntPtr hObject);
        }

        /// <summary>
        /// Drawing.Bitmap を BitmapSource に変換
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static BitmapSource ToBitmapSource(this Bitmap bitmap)
        {
            var hBitmap = bitmap.GetHbitmap();
            try
            {
                var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                bitmapSource.Freeze();
                return bitmapSource;
            }
            finally
            {
                NativeMethods.DeleteObject(hBitmap);
            }
        }
    }
}

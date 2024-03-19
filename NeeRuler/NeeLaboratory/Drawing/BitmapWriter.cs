using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace NeeLaboratory.Drawing
{
    /// <summary>
    /// Bitmap 編集
    /// </summary>
    public class BitmapWriter : IDisposable
    {
        private readonly Bitmap _bitmap;
        private readonly BitmapData _bmpData;
        private readonly int _pixelSize = 4;
        private readonly byte[] _pixels;
        private bool _disposedValue;

        private bool _inverse;
        private bool _transpose;


        public BitmapWriter(Bitmap bitmap, bool inverse, bool transpose)
        {
            Debug.Assert(bitmap.PixelFormat == PixelFormat.Format32bppArgb);

            _bitmap = bitmap;
            _inverse = inverse;
            _transpose = transpose;

            _bmpData = _bitmap.LockBits(
              new Rectangle(0, 0, bitmap.Width, bitmap.Height),
              ImageLockMode.ReadOnly,  // 書き込むときはReadAndWriteで
              bitmap.PixelFormat
            );

            if (_bmpData.Stride < 0) // height * strideで計算するのでマイナスだと困る
            {
                bitmap.UnlockBits(_bmpData);
                throw new Exception();
            }

            _pixels = new byte[_bmpData.Stride * bitmap.Height];
            Marshal.Copy(_bmpData.Scan0, _pixels, 0, _pixels.Length);


            if (_inverse)
            {
                if (_transpose)
                {
                    GetPixelPosition = (int x, int y) => x * _bmpData.Stride + (_bmpData.Width - y - 1) * _pixelSize;
                }
                else
                {
                    GetPixelPosition = (int x, int y) => (_bmpData.Height - y - 1) * _bmpData.Stride + x * _pixelSize;
                }
            }
            else
            {
                if (_transpose)
                {
                    GetPixelPosition = (int x, int y) => x * _bmpData.Stride + y * _pixelSize;
                }
                else
                {
                    GetPixelPosition = (int x, int y) => y * _bmpData.Stride + x * _pixelSize;
                }
            }

            if (_transpose)
            {
                Width = _bmpData.Height;
                Height = _bmpData.Width;
            }
            else
            {
                Width = _bmpData.Width;
                Height = _bmpData.Height;
            }

        }


        public int Width { get; private set; }
        public int Height { get; private set; }

        public byte[] Buffer => _pixels;
        public Func<int, int, int> GetPixelPosition { get; private set; }


        public Color GetPixel(int x, int y)
        {
            int pos = GetPixelPosition(x, y);
            var b = _pixels[pos + 0];
            var g = _pixels[pos + 1];
            var r = _pixels[pos + 2];
            var a = _pixels[pos + 3];
            return Color.FromArgb(a, r, g, b);
        }

        public double GetPixelLuminance(int x, int y)
        {
            int pos = GetPixelPosition(x, y);
            var b = _pixels[pos + 0];
            var g = _pixels[pos + 1];
            var r = _pixels[pos + 2];
            return ((r * 0.3) + (g * 0.59) + (b * 0.11)) / 256.0;
        }

        public void SetPixel(int x, int y, Color color)
        {
            int pos = GetPixelPosition(x, y);
            _pixels[pos + 0] = color.B;
            _pixels[pos + 1] = color.G;
            _pixels[pos + 2] = color.R;
            _pixels[pos + 3] = color.A;
        }

        public void BlendPixel(int x, int y, Color color, double opacity)
        {
            int pos = GetPixelPosition(x, y);
            _pixels[pos + 0] = Linear(_pixels[pos + 0], color.B, opacity);
            _pixels[pos + 1] = Linear(_pixels[pos + 1], color.G, opacity);
            _pixels[pos + 2] = Linear(_pixels[pos + 2], color.R, opacity);
            _pixels[pos + 3] = Linear(_pixels[pos + 3], color.A, opacity); ;
        }

        private int Linear(int a, int b, double rate)
        {
            return (int)(a + (b - a) * rate);
        }

        private byte Linear(byte a, byte b, double rate)
        {
            return (byte)(a + (b - a) * rate);
        }

        public void Flush()
        {
            if (_disposedValue) throw new ObjectDisposedException(this.GetType().FullName);

            Marshal.Copy(_pixels, 0, _bmpData.Scan0, _pixels.Length);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _bitmap.UnlockBits(_bmpData);
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }
}

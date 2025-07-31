using System.Runtime.InteropServices;

namespace NeeRuler.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public int Width
        {
            get => Right - Left;
            set => Right = Left + value;
        }

        public int Height
        {
            get => Bottom - Top;
            set => Bottom = Top + value;
        }
    }

}

using System.Runtime.InteropServices;


namespace NeeRuler.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public readonly bool Equals(POINT other)
        {
            return X == other.X && Y == other.Y;
        }
    }
}

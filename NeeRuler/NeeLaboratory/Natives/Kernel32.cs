using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NeeLaboratory.Natives
{
    internal static partial class NativeMethods
    {
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool SetDllDirectory(string lpPathName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int GetLongPathName(string shortPath, StringBuilder longPath, int longPathLength);

        [DllImport("Kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GlobalAlloc(int uFlags, int dwBytes);

        [DllImport("Kernel32.dll")]
        internal static extern IntPtr GlobalFree(IntPtr hMem);

        [DllImport("Kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("Kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GlobalSize(IntPtr hMem);

        [DllImport("Kernel32.dll")]
        internal static extern bool GlobalUnlock(IntPtr hMem);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern uint FormatMessage(uint dwFlags, IntPtr lpSource, uint dwMessageId, uint dwLanguageId, StringBuilder lpBuffer, int nSize, IntPtr Arguments);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool MoveFile(string lpExistingFileName, string lpNewFileName);
    }
}

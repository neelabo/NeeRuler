using NeeRuler.Interop;
using System;
using System.Windows;
using System.Windows.Interop;


namespace NeeRuler.FollowMouse
{
    public class CursorNativeOffset : NativeOffset
    {
        private readonly Window _window;
        private POINT _offset;

        public CursorNativeOffset(Window window)
        {
            _window = window;
        }

        public override void OnOpen()
        {
            if (!NativeMethods.GetCursorPos(out var point)) return;

            var hwnd = new WindowInteropHelper(_window).Handle;
            if (hwnd == IntPtr.Zero) throw new InvalidOperationException("Cannot get window handle");

            if (!NativeMethods.GetWindowRect(hwnd, out var rect)) return;

            _offset.X = point.X - rect.Left;
            _offset.Y = point.Y - rect.Top;
        }

        public override POINT GetNativeOffset(RECT rect)
        {
            _offset = LimitOffset(_offset, rect, 20);
            return _offset;
        }

        private POINT LimitOffset(POINT offset, RECT rect, int margin)
        {
            if (offset.X < margin) offset.X = margin;
            if (offset.Y < margin) offset.Y = margin;
            if (offset.X > rect.Width - margin) offset.X = rect.Width - margin;
            if (offset.Y > rect.Height - margin) offset.Y = rect.Height - margin;
            return offset;
        }
    }
}

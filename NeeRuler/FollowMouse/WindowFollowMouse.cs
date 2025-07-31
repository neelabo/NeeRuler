using NeeRuler.Interop;
using System;
using System.Data;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;


namespace NeeRuler.FollowMouse
{
    public class WindowFollowMouse
    {
        private readonly Window _window;
        private readonly IntPtr _hwnd;
        private readonly NativeOffset _nativeOffset;
        private readonly Timer _timer;
        private POINT _point;
        private bool _isEnabled;


        public WindowFollowMouse(Window window, NativeOffset? nativeOffset)
        {
            _window = window;
            _nativeOffset = nativeOffset ?? new DefaultNativeOffset();

            _hwnd = new WindowInteropHelper(_window).Handle;
            if (_hwnd == IntPtr.Zero) throw new InvalidOperationException("Cannot get window handle");

            _timer = new Timer();
            _timer.Interval = 10;
            _timer.Elapsed += Timer_Elapsed;
        }


        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (value)
                {
                    Open();
                }
                else
                {
                    Close();
                }
            }
        }

        private void Open()
        {
            if (_isEnabled) return;
            _isEnabled = true;

            _point = new();
            _nativeOffset.OnOpen();

            _timer.Start();
        }

        private void Close()
        {
            if (!_isEnabled) return;
            _isEnabled = false;

            _timer.Stop();

            _nativeOffset.OnClosed();
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            if (!NativeMethods.GetCursorPos(out var point)) return;

            if (point.Equals(_point)) return;
            _point = point;

            if (!NativeMethods.GetWindowRect(_hwnd, out var rect)) return;

            _window.Dispatcher.BeginInvoke((Action)(() =>
            {
                var offset = _nativeOffset.GetNativeOffset(rect);

                NativeMethods.SetWindowPos(_hwnd, IntPtr.Zero,
                    point.X - offset.X,
                    point.Y - offset.Y,
                    0, 0, NativeMethods.SWP_NOZORDER | NativeMethods.SWP_NOSIZE);

                _nativeOffset.OnWindowPositionChanged();

                //Debug.WriteLine($"{_window.Left}, {_window.Top}");
            }));
        }
    }
}

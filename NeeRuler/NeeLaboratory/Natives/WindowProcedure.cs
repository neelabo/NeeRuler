using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;

namespace NeeLaboratory.Natives
{
    internal class WindowProcedure
    {
        private readonly Window _window;
        private readonly Dictionary<WindowMessages, HwndSourceHook> _hooks = new Dictionary<WindowMessages, HwndSourceHook>();

        public WindowProcedure(Window window)
        {
            if (_window != null) throw new ArgumentNullException();
            _window = window;

            if (_window.IsLoaded)
            {
                Attach();
            }
            else
            {
                _window.Loaded += Window_Loaded;
            }
        }

        public void AddHook(WindowMessages message, HwndSourceHook hook)
        {
            _hooks.Add(message, hook);
        }

        public void RemoveHook(WindowMessages message)
        {
            _hooks.Remove(message);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _window.Loaded -= Window_Loaded;
            Attach();
        }

        private void Attach()
        {
            var source = (HwndSource.FromVisual(_window) as HwndSource) ?? throw new InvalidOperationException("Cannot get window handle");
            source.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            try
            {
                if (_hooks.TryGetValue((WindowMessages)msg, out var hook))
                {
                    return hook.Invoke(hwnd, msg, wParam, lParam, ref handled);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return IntPtr.Zero;
        }
    }

}

using NeeRuler.FollowMouse;
using NeeRuler.Interop;
using System;
using System.Windows;
using System.Windows.Media;


namespace NeeRuler.Models
{
    public class BaseLineNativeOffset : NativeOffset
    {
        private readonly Window _window;
        private readonly ReadingRuler _ruler;

        public BaseLineNativeOffset(Window window, ReadingRuler ruler)
        {
            _window = window ?? throw new ArgumentNullException(nameof(window));
            _ruler = ruler;
        }

        public override void OnWindowPositionChanged()
        {
            _ruler.ResetPrevRange();
        }

        public override POINT GetNativeOffset(RECT rect)
        {
            const int margin = 20;

            POINT offset = new();
            offset.X = rect.Width / 2;
            offset.Y = rect.Height / 2;

            var dpi = VisualTreeHelper.GetDpi(_window);
            if (_ruler.IsVertical)
            {
                var delta = Math.Min(Math.Max((int)(_ruler.BaseLine * dpi.DpiScaleX), margin), rect.Width - margin);
                offset.X = rect.Width - delta;
            }
            else
            {
                var delta = Math.Min(Math.Max((int)(_ruler.BaseLine * dpi.DpiScaleY), margin), rect.Height - margin);
                offset.Y = delta;
            }
            return offset;
        }
    }
}

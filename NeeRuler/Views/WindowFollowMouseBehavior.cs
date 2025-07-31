using NeeRuler.FollowMouse;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;


namespace NeeRuler.Views
{
    public class WindowFollowMouseBehavior
    {
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(WindowFollowMouseBehavior), new PropertyMetadata(false, IsEnabled_Changed));

        public static void SetIsEnabled(DependencyObject obj, bool value)
            => obj.SetValue(IsEnabledProperty, value);

        public static bool GetIsEnabled(DependencyObject obj)
            => (bool)obj.GetValue(IsEnabledProperty);


        public static readonly DependencyProperty NativeOffsetProperty =
            DependencyProperty.RegisterAttached("NativeOffset", typeof(NativeOffset), typeof(WindowFollowMouseBehavior), new PropertyMetadata(null, NativeOffset_Changed));


        public static void SetNativeOffset(DependencyObject obj, NativeOffset? value)
            => obj.SetValue(NativeOffsetProperty, value);

        public static NativeOffset? GetNativeOffset(DependencyObject obj)
            => obj.GetValue(NativeOffsetProperty) as NativeOffset;


        public static readonly DependencyProperty InternalParameterProperty =
            DependencyProperty.RegisterAttached("InternalParameter", typeof(WindowFollowMouse), typeof(WindowFollowMouseBehavior), new PropertyMetadata(null));

        private static void SetInternalParameter(DependencyObject obj, WindowFollowMouse? value)
            => obj.SetValue(InternalParameterProperty, value);

        private static WindowFollowMouse? GetInternalParameter(DependencyObject obj)
            => obj.GetValue(InternalParameterProperty) as WindowFollowMouse;


        private static void IsEnabled_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not Window window)
            {
                throw new ArgumentException("WindowFollowMouseBehavior can only be applied to Window types.");
            }

            Update(d);
        }

        private static void NativeOffset_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Update(d);
        }

        private static void Update(DependencyObject d)
        {
            var isEnabled = GetIsEnabled(d);

            if (isEnabled)
            {
                Attach(d);
            }
            else
            {
                Detach(d);
            }
        }

        private static void Attach(DependencyObject d)
        {
            if (d is not Window window)
            {
                throw new ArgumentException("WindowFollowMouseBehavior can only be applied to Window types.");
            }

            var parameter = GetInternalParameter(d);
            if (parameter is not null)
            {
                Detach(d);
            }

            parameter = new WindowFollowMouse(window, GetNativeOffset(d));
            parameter.IsEnabled = true;
            SetInternalParameter(d, parameter);
        }

        private static void Detach(DependencyObject d)
        {
            var parameter = GetInternalParameter(d);
            if (parameter is not null)
            {
                parameter.IsEnabled = false;
                SetInternalParameter(d, null);
            }
        }
    }
}

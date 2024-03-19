using NeeLaboratory.ComponentModel;
using NeeLaboratory.Natives;
using NeeRuler.ViewModels;
using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;


namespace NeeRuler.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _vm;
        private readonly WindowProcedure _windowProcedure;
        private bool _isLeftButtonDown;
        private bool _skipLeftButtonClick;
        private Point _cursorPos;


        public MainWindow()
        {
            InitializeComponent();

            App.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            _vm = new MainWindowViewModel(this.RulerGrid);
            this.DataContext = _vm;

            _windowProcedure = new WindowProcedure(this);
            _windowProcedure.AddHook(WindowMessages.WM_MOUSEACTIVATE, WinProc_MouseActivate);

            var propertyDependency = new PropertyChangedDependency(_vm);
            propertyDependency.Add(nameof(_vm.Width), ViewModel_SizePropertyChanged);
            propertyDependency.Add(nameof(_vm.Height), ViewModel_SizePropertyChanged);

            _vm.Notification += ViewModel_Notification;

            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;
        }


        private void ViewModel_Notification(object sender, NotificationEventArgs e)
        {
            switch (e.MessageType)
            {
                case NotificationType.Information:
                    {
                        this.MessageTextBlock.Text = e.Message;
                        var ani = new DoubleAnimationUsingKeyFrames() { Duration = TimeSpan.FromSeconds(2.0) };
                        ani.KeyFrames.Add(new LinearDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.0))));
                        ani.KeyFrames.Add(new LinearDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1.5))));
                        ani.KeyFrames.Add(new LinearDoubleKeyFrame(0.0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(2.0))));
                        this.MessageBorder.BeginAnimation(OpacityProperty, ani);
                    }
                    break;
                case NotificationType.Warning:
                case NotificationType.Error:
                    NotificationBox.Show(e.Message, e.MessageType);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private void ViewModel_SizePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Width = _vm.Width;
            this.Height = _vm.Height;
        }

        private IntPtr WinProc_MouseActivate(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            _skipLeftButtonClick = true;
            return IntPtr.Zero;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= MainWindow_Loaded;

            _vm.Top = this.Top;
            _vm.Left = this.Left;
            _vm.InitializeStorageLocation();
            this.SetBinding(TopProperty, new Binding(nameof(_vm.Top)) { Mode = BindingMode.TwoWay });
            this.SetBinding(LeftProperty, new Binding(nameof(_vm.Left)) { Mode = BindingMode.TwoWay });
            this.Width = _vm.Width;
            this.Height = _vm.Height;

            _vm.SetDpiScale(VisualTreeHelper.GetDpi(this));
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            _vm.Closed();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _isLeftButtonDown = true;
                _cursorPos = e.GetPosition(this);
            }
            else
            {
                _skipLeftButtonClick = false;
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isLeftButtonDown = true;
            _cursorPos = e.GetPosition(this);
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isLeftButtonDown && !_skipLeftButtonClick)
            {
                _isLeftButtonDown = false;
                _vm.MoveToForward();
            }
            _skipLeftButtonClick = false;
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isLeftButtonDown) return;

            var pos = e.GetPosition(this);
            if (Math.Abs(pos.X - _cursorPos.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(pos.Y - _cursorPos.Y) > SystemParameters.MinimumHorizontalDragDistance)
            {
                _isLeftButtonDown = false;
                _skipLeftButtonClick = false;
                try
                {
                    this.DragMove();
                    _vm.ResetPrevRange();
                    e.Handled = true;
                }
                catch
                {
                }
            }
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            _vm.TextLineTopMargin += e.Delta / 120.0;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Apps)
            {
                this.MainContextMenu.IsOpen = true;
            }

#if DEBUG
            if (e.Key == Key.F12)
            {
                DebugWindow.Show(_vm);
            }
#endif
        }

        private void Window_DpiChanged(object sender, DpiChangedEventArgs e)
        {
            _vm.SetDpiScale(e.NewDpi);
        }

        private void ThumbW_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var oldValue = _vm.Width;
            _vm.Width -= e.HorizontalChange;
            _vm.Left -= _vm.Width - oldValue;
        }

        private void ThumbE_DragDelta(object sender, DragDeltaEventArgs e)
        {
            _vm.Width += e.HorizontalChange;
        }

        private void ThumbN_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var oldValue = _vm.Height;
            _vm.Height -= e.VerticalChange;
            _vm.Top -= _vm.Height - oldValue;
        }

        private void ThumbS_DragDelta(object sender, DragDeltaEventArgs e)
        {
            _vm.Height += e.VerticalChange;
        }

        private void ThumbNW_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ThumbN_DragDelta(sender, e);
            ThumbW_DragDelta(sender, e);
        }

        private void ThumbNE_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ThumbN_DragDelta(sender, e);
            ThumbE_DragDelta(sender, e);
        }

        private void ThumbSW_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ThumbS_DragDelta(sender, e);
            ThumbW_DragDelta(sender, e);
        }

        private void ThumbSE_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ThumbS_DragDelta(sender, e);
            ThumbE_DragDelta(sender, e);
        }

        private void MenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            _vm.UpdateProfileItems();
        }
    }
}

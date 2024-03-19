using NeeRuler.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NeeRuler.Views
{
    /// <summary>
    /// DebugWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class DebugWindow : Window
    {
        private static DebugWindow? _window;
        
        public static void Show(MainWindowViewModel vm)
        {
            if (_window is not null)
            {
                _window.Activate();
                return;
            }

            _window = new DebugWindow(vm);
            _window.Show();
        }


        private readonly MainWindowViewModel? _vm;

        public DebugWindow()
        {
            InitializeComponent();
        }

        public DebugWindow(MainWindowViewModel vm) : this()
        {
            _vm = vm;
            _vm.PropertyChanged += ViewModel_PropertyChanged;

            this.DataContext = vm;

            Closed += DebugWindow_Closed;
        }

        private void DebugWindow_Closed(object sender, EventArgs e)
        {
            if (_vm is null) return;

            _vm.PropertyChanged -= ViewModel_PropertyChanged;
            _window = null;
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_vm is null) return;
            switch (e.PropertyName)
            {
                case nameof(MainWindowViewModel.CaptureBitmap):
                    this.CaptureBitmapRotate.Angle = _vm.IsVertical ? -90 : 0;
                    break;
            }
        }

    }

}

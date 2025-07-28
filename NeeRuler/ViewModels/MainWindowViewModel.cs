using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using NeeLaboratory.ComponentModel;
using NeeLaboratory.Windows.Input;
using NeeRuler.Models;

namespace NeeRuler.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly List<MenuItemSource> _emptyMenuItems = new() { new MenuItemSource("None") { IsEnabled = false } };
        private readonly ReadingRuler _ruler;
        private readonly AppCommandMap _commandMap;


        public MainWindowViewModel() : this(null)
        {
        }

        public MainWindowViewModel(FrameworkElement? rulerGrid)
        {
            _ruler = new ReadingRuler(AppContext.Current, rulerGrid);

            _commandMap = new AppCommandMap(AppContext.Current, _ruler);
            _commandMap.LoadKeyConfig();
            if (rulerGrid != null)
            {
                _commandMap.AddInputBinding(Window.GetWindow(rulerGrid));
            }

            var propertyDependency = CreatePropertyChangedDependency(_ruler);

            propertyDependency.Add(nameof(_ruler.Top), nameof(Top));
            propertyDependency.Add(nameof(_ruler.Left), nameof(Left));
            propertyDependency.Add(nameof(_ruler.Width), nameof(Width), nameof(Height));
            propertyDependency.Add(nameof(_ruler.Height), nameof(Height), nameof(Width));
            propertyDependency.Add(nameof(_ruler.TextLineHeight), nameof(TextLineHeight), nameof(TextAreaHeight), nameof(TopAreaHeight));
            propertyDependency.Add(nameof(_ruler.TextLineTopMargin), nameof(TextLineTopMargin), nameof(TextAreaHeight), nameof(TopAreaHeight));
            propertyDependency.Add(nameof(_ruler.TextLineBottomMargin), nameof(TextLineBottomMargin), nameof(TextAreaHeight), nameof(TopAreaHeight));
            propertyDependency.Add(nameof(_ruler.IsFlatPanel), nameof(IsFlatPanel), nameof(BackgroundColor));
            propertyDependency.Add(nameof(_ruler.BaseLine), nameof(BaseLine), nameof(TextAreaHeight), nameof(TopAreaHeight));
            propertyDependency.Add(nameof(_ruler.BaseLineHeight), nameof(BaseLineHeight));
            propertyDependency.Add(nameof(_ruler.BackgroundColor), nameof(BackgroundColor));
            propertyDependency.Add(nameof(_ruler.TextLineColor), nameof(TextLineColor), nameof(BackgroundColor));
            propertyDependency.Add(nameof(_ruler.BaseLineColor), nameof(BaseLineColor));
            propertyDependency.Add(nameof(_ruler.IsAutoStride), nameof(IsAutoStride));
            propertyDependency.Add(nameof(_ruler.IsVertical), nameof(IsVertical), nameof(LayoutAngle), nameof(Width), nameof(Height));
            propertyDependency.Add(nameof(_ruler.CaptureBitmap), nameof(CaptureBitmap));
            propertyDependency.Add(nameof(_ruler.ProfileIds), nameof(ProfileMenuItems));
            propertyDependency.Add(nameof(_ruler.InactiveWindowOpacity), nameof(InactiveWindowOpacity));

            AppCommand = new RelayCommand<string>(e => _commandMap.ExecuteCommand(e, null));
            AboutCommand = new RelayCommand(_ruler.OpenAbout);
        }


        public void UpdateProfileItems()
        {
            _ruler.UpdateProfileIds();
        }

        public event EventHandler<NotificationEventArgs> Notification
        {
            add => _ruler.Notification += value;
            remove => _ruler.Notification -= value;
        }



        public BitmapSource? CaptureBitmap
        {
            get => _ruler.CaptureBitmap;
        }

        public double Top
        {
            get => _ruler.Top;
            set => _ruler.Top = value;
        }

        public double Left
        {
            get => _ruler.Left;
            set => _ruler.Left = value;
        }

        public double Width
        {
            get => _ruler.IsVertical ? _ruler.Height : _ruler.Width;
            set
            {
                if (_ruler.IsVertical)
                    _ruler.Height = value;
                else
                    _ruler.Width = value;
            }
        }

        public double Height
        {
            get => _ruler.IsVertical ? _ruler.Width : _ruler.Height;
            set
            {
                if (_ruler.IsVertical)
                    _ruler.Width = value;
                else
                    _ruler.Height = value;
            }
        }

        public double TextLineHeight
        {
            get => _ruler.TextLineHeight;
            set => _ruler.TextLineHeight = value;
        }

        public double TextLineTopMargin
        {
            get => _ruler.TextLineTopMargin;
            set => _ruler.TextLineTopMargin = value;
        }

        public double TextLineBottomMargin
        {
            get => _ruler.TextLineBottomMargin;
            set => _ruler.TextLineBottomMargin = value;
        }
        
        public bool IsFlatPanel
        {
            get => _ruler.IsFlatPanel;
            set => _ruler.IsFlatPanel = value;
        }

        public double BaseLine
        {
            get => _ruler.BaseLine;
            set => _ruler.BaseLine = value;
        }

        public double BaseLineHeight
        {
            get => _ruler.BaseLineHeight;
            set => _ruler.BaseLineHeight = value;
        }

        public double TextAreaHeight
        {
            get => Math.Min(TextLineHeight + TextLineTopMargin + TextLineBottomMargin, BaseLine);
        }

        public double TopAreaHeight
        {
            get => Math.Max(BaseLine - TextAreaHeight, 0.0);
        }


        public bool IsAutoStride
        {
            get => _ruler.IsAutoStride;
            set => _ruler.IsAutoStride = value;
        }

        public bool IsVertical
        {
            get => _ruler.IsVertical;
            set => _ruler.IsVertical = value;
        }


        public double LayoutAngle
        {
            get => IsVertical ? 90.0 : 0.0;
        }


        public Color BackgroundColor
        {
            get => _ruler.IsFlatPanel ? _ruler.TextLineColor : _ruler.BackgroundColor;
        }

        public Color TextLineColor
        {
            get => _ruler.TextLineColor;
        }

        public Color BaseLineColor
        {
            get => _ruler.BaseLineColor;
        }

        public List<MenuItemSource> ProfileMenuItems
        {
            get
            {
                var menuItems = _ruler.ProfileIds.Select(e => CreateProfileMenuItem(e)).ToList();
                return menuItems.Any() ? menuItems : _emptyMenuItems;
            }
        }

        public double InactiveWindowOpacity
        {
            get => _ruler.InactiveWindowOpacity;
            set => _ruler.InactiveWindowOpacity = value;
        }


        public RelayCommand<string> AppCommand { get; }
        public RelayCommand AboutCommand { get; }



        private MenuItemSource CreateProfileMenuItem(string id)
        {
            return new MenuItemSource($"Profile-_{id}", new ProfileCommand(_ruler, id));
        }

        public void InitializeStorageLocation()
        {
            _ruler.InitializeStorageLocation();
        }

        public void SetDpiScale(DpiScale dpiScale)
        {
            _ruler.DpiScale = dpiScale;
        }

        public void ResetPrevRange()
        {
            _ruler.ResetPrevRange();
        }

        public void Closed()
        {
            _ruler.Dispose();
        }

        public void MoveToForward()
        {
            if (_ruler.IsVertical)
            {
                _ruler.MoveLeft();
            }
            else
            {
                _ruler.MoveDown();
            }
        }
    }
}
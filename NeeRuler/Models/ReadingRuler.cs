using NeeLaboratory.ComponentModel;
using NeeLaboratory.Linq;
using NeeLaboratory.Windows.Media;
using NeeRuler.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace NeeRuler.Models
{
    public class ReadingRuler : BindableBase, IDisposable
    {
        private double _top;
        private double _left;
        private double _width = 1024.0;
        private double _height = 256.0;
        private bool _isFlatPanel = false;
        private double _inactiveWindowOpacity = 0.5;
        private double _textLineHeight = 32.0;
        private double _textLineTopMargin = 4.0;
        private double _textLineBottomMargin = 4.0;
        private double _baseLine = 64.0;
        private double _baseLineHeight = 2.0;
        private Color _backgroundColor = ColorUtils.FromCode(0xAA4682B4);
        private Color _textLineColor = ColorUtils.FromCode(0x444682B4);
        private Color _baseLineColor = ColorUtils.FromCode(0xAA000000);
        private bool _isAutoStride = true;
        private bool _isVertical;
        private bool _isFollowMouse;
        private double _stride = 32.0;
        private double _adjustmentStride = 2.0;
        private DpiScale _dpiScale = new DpiScale(1.0, 1.0);

        private readonly AppContext _appContext;
        private readonly AutoStride _autoStride;
        private double? _storageLocationX;
        private double? _storageLocationY;
        private List<string> _profileIds = new List<string>();
        private bool _disposedValue;

        public ReadingRuler(AppContext appContext, FrameworkElement? rulerGrid)
        {
            _appContext = appContext;
            _autoStride = new AutoStride(this, rulerGrid);

            ImportProfile(_appContext.Settings);

            var propertyDependency = CreatePropertyChangedDependency(_autoStride);
            propertyDependency.Add(nameof(_autoStride.CaptureBitmap), nameof(CaptureBitmap));
        }


        public event EventHandler<NotificationEventArgs>? Notification;


        public BitmapSource? CaptureBitmap
        {
            get => _autoStride.CaptureBitmap;
        }

        public Point StorageLocation
        {
            get { return new Point(_storageLocationX ?? 0.0, _storageLocationY ?? 0.0); }
            set
            {
                if (_storageLocationX != value.X || _storageLocationY != value.Y)
                {
                    _storageLocationX = value.X;
                    _storageLocationY = value.Y;
                    RaisePropertyChanged();
                }
            }
        }

        public double Top
        {
            get { return _top; }
            set { SetProperty(ref _top, value); }
        }

        public double Left
        {
            get { return _left; }
            set { SetProperty(ref _left, value); }
        }

        public double Width
        {
            get { return _width; }
            set { SetProperty(ref _width, Math.Max(value, 64.0)); }
        }

        public bool IsFlatPanel
        {
            get { return _isFlatPanel; }
            set { SetProperty(ref _isFlatPanel, value); }
        }

        public double Height
        {
            get { return _height; }
            set { SetProperty(ref _height, Math.Max(value, 64.0)); }
        }

        public double TextLineHeight
        {
            get { return _textLineHeight; }
            set { SetProperty(ref _textLineHeight, Math.Max(value, 0.0)); }
        }

        public double TextLineTopMargin
        {
            get { return _textLineTopMargin; }
            set { SetProperty(ref _textLineTopMargin, Math.Max(value, 0.0)); }
        }

        public double TextLineBottomMargin
        {
            get { return _textLineBottomMargin; }
            set { SetProperty(ref _textLineBottomMargin, Math.Max(value, 0.0)); }
        }

        public double BaseLine
        {
            get { return _baseLine; }
            set { SetProperty(ref _baseLine, Math.Max(value, 0.0)); }
        }

        public double BaseLineHeight
        {
            get { return _baseLineHeight; }
            set { SetProperty(ref _baseLineHeight, value); }
        }

        public Color BackgroundColor
        {
            get { return _backgroundColor; }
            set { SetProperty(ref _backgroundColor, value); }
        }

        public Color TextLineColor
        {
            get { return _textLineColor; }
            set { SetProperty(ref _textLineColor, value); }
        }

        public Color BaseLineColor
        {
            get { return _baseLineColor; }
            set { SetProperty(ref _baseLineColor, value); }
        }

        public bool IsAutoStride
        {
            get { return _isAutoStride; }
            set { SetProperty(ref _isAutoStride, value); }
        }

        public bool IsVertical
        {
            get { return _isVertical; }
            set { SetProperty(ref _isVertical, value); }
        }

        public bool IsFollowMouse
        {
            get { return _isFollowMouse; }
            set { SetProperty(ref _isFollowMouse, value); }
        }

        public double Stride
        {
            get { return _stride; }
            set { SetProperty(ref _stride, value); }
        }

        public double AdjustmentStride
        {
            get { return _adjustmentStride; }
            set { SetProperty(ref _adjustmentStride, value); }
        }

        public DpiScale DpiScale
        {
            get { return _dpiScale; }
            set { SetProperty(ref _dpiScale, value); }
        }

        public List<string> ProfileIds
        {
            get { return _profileIds; }
            set { SetProperty(ref _profileIds, value); }
        }

        public double InactiveWindowOpacity
        {
            get { return _inactiveWindowOpacity; }
            set { SetProperty(ref _inactiveWindowOpacity, value); }
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _appContext.Settings = CreateProfile();
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void InitializeStorageLocation()
        {
            _storageLocationX ??= Left;
            _storageLocationY ??= Top;
        }

        public void ToggleIsAutoStride()
        {
            IsAutoStride = !IsAutoStride;
            Notify(string.Format(TextResources.GetString("Notify.AutoStride"), (_isAutoStride ? TextResources.GetString("Word.On") : TextResources.GetString("Word.Off"))));
        }

        public void ToggleIsVertical()
        {
            IsVertical = !IsVertical;
        }

        public void ToggleIsFlatPanel()
        {
            IsFlatPanel = !IsFlatPanel;
        }

        public void ToggleIsFollowMouse()
        {
            IsFollowMouse = !IsFollowMouse;
        }

        public void StoreLocation()
        {
            Notify(TextResources.GetString("Notify.StoreLocation"));
            _storageLocationX = Left;
            _storageLocationY = Top;
        }

        public void RestoreLocation()
        {
            if (_storageLocationX is null || _storageLocationY is null) return;
            //Notify("Restore location");
            this.Left = _storageLocationX ?? 0.0;
            this.Top = _storageLocationY ?? 0.0;
        }

        public void ResetPrevRange()
        {
            _autoStride.ResetPrevRange();
        }

        private Range GetDefaultRange()
        {
            return new Range(8, Math.Max((int)Stride - 8, 8));
        }

        private void Notify(string message, NotificationType messageType = NotificationType.Information)
        {
            Notification?.Invoke(this, new NotificationEventArgs(message, messageType));
        }

        public void MoveDown()
        {
            if (IsAutoStride && !IsVertical)
            {
                var result = _autoStride.MoveToNextTextLine(+1, false, GetDefaultRange(), _dpiScale);
                this.TextLineHeight = result.TextHeight;
                this.Top += result.Delta / _dpiScale.DpiScaleY;
            }
            else
            {
                this.Top += _stride / _dpiScale.DpiScaleY;
            }
        }

        public void MoveUp()
        {
            if (IsAutoStride && !IsVertical)
            {
                var result = _autoStride.MoveToNextTextLine(-1, false, GetDefaultRange(), _dpiScale);
                this.TextLineHeight = result.TextHeight;
                this.Top += result.Delta / _dpiScale.DpiScaleY;
            }
            else
            {
                this.Top += -_stride / _dpiScale.DpiScaleY;
            }
        }

        public void MoveLeft()
        {
            if (IsAutoStride && IsVertical)
            {
                var result = _autoStride.MoveToNextTextLine(+1, true, GetDefaultRange(), _dpiScale);
                this.TextLineHeight = result.TextHeight;
                this.Left -= result.Delta / _dpiScale.DpiScaleX;
            }
            else
            {
                this.Left -= _stride / _dpiScale.DpiScaleX;
            }
        }

        public void MoveRight()
        {
            if (IsAutoStride && IsVertical)
            {
                var result = _autoStride.MoveToNextTextLine(-1, true, GetDefaultRange(), _dpiScale);
                this.TextLineHeight = result.TextHeight;
                this.Left -= result.Delta / _dpiScale.DpiScaleX;
            }
            else
            {
                this.Left -= -_stride / _dpiScale.DpiScaleX;
            }
        }



        public void AdjustUp()
        {
            this.Top -= _adjustmentStride / _dpiScale.DpiScaleY;
        }

        public void AdjustDown()
        {
            this.Top += _adjustmentStride / _dpiScale.DpiScaleY;
        }

        public void AdjustLeft()
        {
            this.Left -= _adjustmentStride / _dpiScale.DpiScaleX;
        }

        public void AdjustRight()
        {
            this.Left += _adjustmentStride / _dpiScale.DpiScaleX;
        }

        public void ImportProfile(string id)
        {
            var name = $"Profile-{id}";
            Notify(name);
            try
            {
                ImportProfileFile(Path.Combine(_appContext.ProfileDirectory, $"{name}.json"));
            }
            catch (Exception ex)
            {
                var message = string.Format(TextResources.GetString("Notify.FailedToLoadFile"), $"{name}.json") + "\n\n" + ex.Message;
                Notify(message, NotificationType.Error);
            }
        }

        public void OpenProfilesFolder()
        {
            try
            {
                var startInfo = new ProcessStartInfo("explorer.exe", _appContext.ProfileDirectory)
                {
                    UseShellExecute = false,
                };
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                Notify(ex.Message, NotificationType.Error);
            }
        }

        public void OpenAbout()
        {
            try
            {
                var fileName = IsJapaneseCulture(TextResources.Culture) ? "Readme.ja-jp.html" : "Readme.html";
                var path = Path.Combine(Path.GetDirectoryName(_appContext.AssemblyLocation), fileName);
                var startInfo = new ProcessStartInfo(path)
                {
                    UseShellExecute = true,
                };
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                Notify(ex.Message, NotificationType.Error);
            }
        }

        private bool IsJapaneseCulture(CultureInfo culture)
        {
            if (culture.Equals(CultureInfo.InvariantCulture)) return false;

            return culture.Equals(CultureInfo.GetCultureInfo("ja")) ? true : IsJapaneseCulture(culture.Parent);
        }

        public void Exit()
        {
            Application.Current.Shutdown();
        }

        public void UpdateProfileIds()
        {
            ProfileIds = GetProfileIds(_appContext.ProfileDirectory);
        }

        public static List<string> GetProfileIds(string profileDirectory)
        {
            if (!Directory.Exists(profileDirectory)) return new List<string>();

            return Directory.GetFiles(profileDirectory, "Profile-*.json")
                .Select(e => GetProfileId(e))
                .WhereNotNull()
                .ToList();
        }

        private static string? GetProfileId(string path)
        {
            var regex = new Regex(@"^Profile-(.+)\.json$");
            var match = regex.Match(Path.GetFileName(path));
            return match.Success ? match.Groups[1].Value : null;
        }


        public Profile CreateProfile()
        {
            return new Profile()
            {
                IsAutoStride = IsAutoStride,
                Stride = Stride,
                AdjustmentStride = AdjustmentStride,
                StorageLocationX = _storageLocationX,
                StorageLocationY = _storageLocationY,

                Layout = new LayoutProfile()
                {
                    BackgroundColor = BackgroundColor,
                    TextLineColor = TextLineColor,
                    BaseLineColor = BaseLineColor,
                    Width = Width,
                    Height = Height,
                    TextLineHeight = TextLineHeight,
                    TextLineTopMargin = TextLineTopMargin,
                    TextLineBottomMargin = TextLineBottomMargin,
                    BaseLine = BaseLine,
                    BaseLineHeight = BaseLineHeight,
                    IsVertical = IsVertical,
                    IsFlatPanel = IsFlatPanel,
                    InactiveWindowOpacity = InactiveWindowOpacity,
                    IsFollowMouse = IsFollowMouse,
                },

                AutoStride = new AutoStrideProfile()
                {
                    TextLineComplexityThreshold = _autoStride.Analyzer.TextLineComplexityThreshold,
                    MinTextHeight = _autoStride.Analyzer.MinTextLineHeight,
                }
            };
        }


        private void ExportProfileFile(string filePath)
        {
            var profile = CreateProfile();
            JsonConverter.Serialize(profile, filePath);
        }


        private void ImportProfileFile(string filePath)
        {
            var profile = JsonConverter.Deserialize<Profile>(filePath);
            ImportProfile(profile);
        }

        private void ImportProfile(Profile? profile)
        {
            if (profile == null) return;

            IsAutoStride = profile.IsAutoStride ?? IsAutoStride;
            Stride = profile.Stride ?? Stride;
            AdjustmentStride = profile.AdjustmentStride ?? AdjustmentStride;
            _storageLocationX = profile.StorageLocationX ?? _storageLocationX;
            _storageLocationY = profile.StorageLocationY ?? _storageLocationX;


            if (profile.Layout is LayoutProfile layout)
            {
                BackgroundColor = layout.BackgroundColor ?? BackgroundColor;
                TextLineColor = layout.TextLineColor ?? TextLineColor;
                BaseLineColor = layout.BaseLineColor ?? BaseLineColor;
                Width = layout.Width ?? Width;
                Height = layout.Height ?? Height;
                TextLineHeight = layout.TextLineHeight ?? TextLineHeight;
                TextLineTopMargin = layout.TextLineTopMargin ?? TextLineTopMargin;
                TextLineBottomMargin = layout.TextLineBottomMargin ?? TextLineBottomMargin;
                BaseLine = layout.BaseLine ?? BaseLine;
                BaseLineHeight = layout.BaseLineHeight ?? BaseLineHeight;
                IsVertical = layout.IsVertical ?? IsVertical;
                IsFlatPanel = layout.IsFlatPanel ?? IsFlatPanel;
                InactiveWindowOpacity = layout.InactiveWindowOpacity ?? InactiveWindowOpacity;
                IsFollowMouse = layout.IsFollowMouse ?? IsFollowMouse;
            }

            if (profile.AutoStride is AutoStrideProfile autoStride)
            {
                _autoStride.Analyzer.TextLineComplexityThreshold = autoStride.TextLineComplexityThreshold ?? _autoStride.Analyzer.TextLineComplexityThreshold;
                _autoStride.Analyzer.MinTextLineHeight = (int)(autoStride.MinTextHeight ?? _autoStride.Analyzer.MinTextLineHeight);
            }
        }

    }

}
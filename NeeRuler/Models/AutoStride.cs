using NeeLaboratory.ComponentModel;
using NeeLaboratory.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace NeeRuler.Models
{
    public class AutoStride : BindableBase
    {
        private readonly ReadingRuler _ruler;
        private readonly FrameworkElement? _rulerGrid;
        private readonly TextLineAnalyzer _analyzer = new TextLineAnalyzer();
        private Range _prevRange;

        private BitmapSource? _captureBitmap;


        public AutoStride(ReadingRuler ruler, FrameworkElement? rulerGrid)
        {
            _ruler = ruler;
            _rulerGrid = rulerGrid;
        }


        public TextLineAnalyzer Analyzer
        {
            get => _analyzer;
        }


        public BitmapSource? CaptureBitmap
        {
            get { return _captureBitmap; }
            set { SetProperty(ref _captureBitmap, value); }
        }



        public void ResetPrevRange()
        {
            _prevRange = Range.Empty;
        }


        public AutoStrideResult MoveToNextTextLine(int direction, bool vertical, Range defaultRange, DpiScale dpiScale)
        {
            Debug.Assert(direction == -1 || direction == +1);

            if (_rulerGrid is null) return new AutoStrideResult();

            var captureOffset = direction < 0 ? -_prevRange.Length : 0;
            var captureY = _ruler.BaseLine - _ruler.TextLineBottomMargin + captureOffset;

            var range = Capture(_rulerGrid, direction, vertical, captureY, dpiScale);

            if (_prevRange.IsEmpty)
            {
                _prevRange = defaultRange;
            }

            // 予測範囲が次の行の手前に存在しうるなら予測範囲を優先する
            if (range.IsEmpty || _prevRange.To + _ruler.TextLineBottomMargin < range.From)
            {
                range = _prevRange;
            }

            _prevRange = range;

            return new AutoStrideResult(captureOffset + (direction < 0 ? -range.From : range.To), range.Length);
        }


        private Range Capture(FrameworkElement element, int direction, bool vertical, double y, DpiScale dpiScale)
        {
            Debug.Assert(direction == -1 || direction == +1);

            if (element is null) return Range.Empty;

            double width = element.ActualWidth;
            double height = 256.0;

            // スクショエリアの座標を取得
            var targetPoint = element.PointToScreen(new Point(0.0, y));

            double dx = 0.0;
            double dy = 0.0;
            double captureWidth;
            double captureHeight;

            if (vertical)
            {
                dx = direction < 0 ? 0 : -height;
                captureWidth = height;
                captureHeight = width;
            }
            else
            {
                dy = direction < 0 ? -height : 0;
                captureWidth = width;
                captureHeight = height;
            }

            // キャプチャ領域の生成
            var targetRect = new Rect(targetPoint.X + dx, targetPoint.Y + dy, captureWidth, captureHeight);

            // スクリーンショット実行
            var bitmap = DrawingUtils.CaptureScreen(targetRect);

            // analyze
            var inverse = (direction < 0) ^ vertical;
            var lines = _analyzer.Analyze(bitmap, inverse, vertical, dpiScale);

            this.CaptureBitmap = bitmap.ToBitmapSource();

            var line = lines.FirstOrDefault(e => e.From > 0);
            return line;
        }
    }
}
using NeeLaboratory.Drawing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows;


namespace NeeRuler.Models
{
    public class TextLineAnalyzer
    {
        /// <summary>
        /// トリミングする複雑度のしきい値
        /// </summary>
        public double TrimComplexityThreshold { get; } = 2.0;
        
        /// <summary>
        /// 複雑度の上限
        /// </summary>
        public double TextLineComplexityMax { get; set; } = 100.0;

        /// <summary>
        /// テキストラインと判定する複雑度しきい値
        /// </summary>
        public double TextLineComplexityThreshold { get; set; } = 0.1;

        /// <summary>
        /// テキストラインと判定する最小高
        /// </summary>
        public int MinTextLineHeight { get; set; } = 8;


        /// <summary>
        /// 画像からテキスト行を抽出する
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public List<Range> Analyze(Bitmap bitmap, bool inverse, bool transpose, DpiScale dpiScale)
        {
            using (var bmp = new BitmapWriter(bitmap, inverse, transpose))
            {
                // trim left
                var x0 = GetTrimLeft(bmp);

                // calc limit
                var x1 = Math.Min(x0 + 512, bmp.Width);

                // calc complexity
                var complexities = GetComplexities(bmp, x0, x1);

                // normalize
                complexities = NormalizeComplexities(complexities);

                // search text line
                var lines = GetTextLine(bmp, complexities, (transpose ? dpiScale.DpiScaleX : dpiScale.DpiScaleY));

                // write debug info
                WriteDebugInfo(bmp, x0, x1, complexities, lines);

                return lines;
            }
        }

        private int GetTrimLeft(BitmapWriter bmp)
        {
            var max = Math.Max(bmp.Width - 256, 0);

            for (int x = 0; x < max; x++)
            {
                var complexity = new GradientComplexityCounter();
                complexity.Initialize(bmp.GetPixelLuminance(x, 0));
                for (int y = 1; y < bmp.Height; y++)
                {
                    complexity.Add(bmp.GetPixelLuminance(x, y));
                    if (complexity.Complexity > TrimComplexityThreshold)
                    {
                        return x;
                    }
                }
            }

            return max;
        }

        private double[] GetComplexities(BitmapWriter bmp, int x0, int x1)
        {
            var complexities = new double[bmp.Height];

            // complexity method
            var complexity = new GradientComplexityCounter(); 
            //var complexity = new DeltaComplexityCounter();
            
            for (int y = 0; y < bmp.Height; y++)
            {
                complexity.Initialize(bmp.GetPixelLuminance(x0, y));
                for (int x = x0 + 1; x < x1; x++)
                {
                    complexity.Add(bmp.GetPixelLuminance(x, y));
                }
                complexities[y] = complexity.Complexity;
            }

            return complexities;
        }

        private double[] NormalizeComplexities(double[] complexities)
        {
            var min = complexities.Min();
            var normalized = complexities.Select(e => Math.Min((e - min) / TextLineComplexityMax, 1.0));
            return normalized.ToArray();
        }

        private List<Range> GetTextLine(BitmapWriter bmp, double[] complexities, double dpiScale)
        {
            var lines = new List<Range>();
            var textLineState = false;
            var textLineFrom = 0;
            int length = 0;

            for (int y = 0; y < bmp.Height; y++)
            {
                var isTextLine = complexities[y] > TextLineComplexityThreshold;

                if (textLineState)
                {
                    length++;

                    if (isTextLine)
                    {
                    }
                    else
                    {
                        if (length > MinTextLineHeight * dpiScale)
                        {
                            lines.Add(new Range(textLineFrom, y));
                        }
                        length = 0;
                        textLineState = false;
                    }
                }
                else
                {
                    length++;

                    if (isTextLine)
                    {
                        textLineFrom = y;
                        length = 0;
                        textLineState = true;
                    }
                    else
                    {
                    }
                }
            }

            return lines;
        }

        // write debug info to bitmap
        [Conditional("DEBUG")]
        private void WriteDebugInfo(BitmapWriter bmp, int x0, int x1, double[] complexities, List<Range> lines)
        {
            foreach (var line in lines)
            {
                for (int y = line.From; y < line.To; y++)
                {
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        bmp.BlendPixel(x, y, Color.Red, 0.5);
                    }
                }
            }

            for (int y = 0; y < bmp.Height; y++)
            {
                bmp.BlendPixel(x0, y, Color.Green, 0.5);

                bmp.BlendPixel(x1 - 1, y, Color.Green, 0.5);

                var x = (int)(complexities[y] * 100.0);
                var c = (complexities[y] < TextLineComplexityThreshold) ? Color.Blue : Color.Red;
                bmp.SetPixel(x + 0, y, c);
                bmp.SetPixel(x + 1, y, c);
            }

            bmp.Flush();
        }

    }
}

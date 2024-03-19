using System;
using System.Diagnostics;

namespace NeeRuler.Models
{
    /// <summary>
    /// 勾配方向での複雑度計測
    /// </summary>
    public class GradientComplexityCounter : IComplexityCounter
    {
        private double _oldValue;
        private double _oldDelta;
        private double _complexity;

        /// <summary>
        /// 勾配とみなす変化量
        /// </summary>
        public double DeltaThreshold { get; set; } = 0.01;


        public double Complexity => _complexity;


        public void Initialize(double value)
        {
            _complexity = 0.0;
            _oldValue = value;
            _oldDelta = -1.0;
        }

        public void Add(double value)
        {
            Debug.Assert(0.0 <= value && value <= 1.0);

           // ちょこっと平均化
           value = (_oldValue + value) / 2;

            var delta = value - _oldValue;
            if (Math.Abs(delta) > DeltaThreshold)
            {
                Debug.Assert(_oldDelta != 0.0);
                if (delta * _oldDelta < 0.0)
                {
                    _complexity += 1.0;
                    _oldDelta = delta;
                }
                _oldValue = value;
            }
        }
    }
}

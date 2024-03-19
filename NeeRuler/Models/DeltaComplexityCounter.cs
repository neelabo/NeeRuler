using System;

namespace NeeRuler.Models
{
    /// <summary>
    /// 差分量での複雑度計測
    /// </summary>
    public class DeltaComplexityCounter : IComplexityCounter
    {
        private double _oldValue;
        private double _complexity;


        public double Complexity => _complexity;


        public void Initialize(double value)
        {
            _complexity = 0.0;
            _oldValue = value;
        }

        public void Add(double value)
        {
            var delta = value - _oldValue;
            _complexity += Math.Abs(delta);
            _oldValue = value;

        }
    }
}

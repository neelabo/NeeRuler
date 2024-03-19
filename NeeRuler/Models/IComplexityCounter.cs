namespace NeeRuler.Models
{
    /// <summary>
    /// 複雑度計測
    /// </summary>
    public interface IComplexityCounter
    {
        /// <summary>
        /// 複雑度
        /// </summary>
        /// <remarks>
        /// おおよその値の切り替え回数
        /// </remarks>
        double Complexity { get; }

        /// <summary>
        /// 初期計測値
        /// </summary>
        /// <param name="value">計測値 [0.0-1.0]</param>
        void Initialize(double value);

        /// <summary>
        /// 計測値追加
        /// </summary>
        /// <remarks>
        /// この計測値と前回の計測値の差分を変化量として複雑度を計算する。
        /// </remarks>
        /// <param name="value">計測値 [0.0-1.0]</param>
        void Add(double value);
    }
}

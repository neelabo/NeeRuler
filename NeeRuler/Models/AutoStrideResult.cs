namespace NeeRuler.Models
{
    public class AutoStrideResult
    {
        public AutoStrideResult()
        {
        }

        public AutoStrideResult(double delta, double textHeight)
        {
            Delta = delta;
            TextHeight = textHeight;
        }

        public double Delta { get; }
        public double TextHeight { get; }
    }
}
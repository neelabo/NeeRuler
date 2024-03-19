namespace NeeRuler.Models
{
    public struct Range
    {
        public static Range Empty { get; } = new Range(0, 0);

        public Range(int from, int to)
        {
            From = from;
            To = to;
        }

        public int From { get; private set; }
        public int To { get; private set; }
        public int Length => To - From;
        public bool IsEmpty => From == 0 && To == 0;

        public override string ToString()
        {
            return IsEmpty ? "Empty" : $"From={From}, To={To}";
        }
    }
}

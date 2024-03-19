namespace NeeRuler.Models
{
    public class AdjustDownCommand : ReadingRulerCommand
    {
        public AdjustDownCommand(ReadingRuler ruler) : base(ruler)
        {
        }

        public override void Execute(object parameter)
        {
            _ruler.AdjustDown();
        }
    }
}
namespace NeeRuler.Models
{
    public class AdjustUpCommand : ReadingRulerCommand
    {
        public AdjustUpCommand(ReadingRuler ruler) : base(ruler)
        {
        }

        public override void Execute(object parameter)
        {
            _ruler.AdjustUp();
        }
    }
}
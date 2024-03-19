namespace NeeRuler.Models
{
    public class AdjustRightCommand : ReadingRulerCommand
    {
        public AdjustRightCommand(ReadingRuler ruler) : base(ruler)
        {
        }

        public override void Execute(object parameter)
        {
            _ruler.AdjustRight();
        }
    }
}
namespace NeeRuler.Models
{
    public class AdjustLeftCommand : ReadingRulerCommand
    {
        public AdjustLeftCommand(ReadingRuler ruler) : base(ruler)
        {
        }

        public override void Execute(object parameter)
        {
            _ruler.AdjustLeft();
        }
    }
}
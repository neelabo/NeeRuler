namespace NeeRuler.Models
{
    public class MoveRightCommand : ReadingRulerCommand
    {
        public MoveRightCommand(ReadingRuler ruler) : base(ruler)
        {
        }

        public override void Execute(object parameter)
        {
            _ruler.MoveRight();
        }
    }
}
namespace NeeRuler.Models
{
    public class MoveLeftCommand : ReadingRulerCommand
    {
        public MoveLeftCommand(ReadingRuler ruler) : base(ruler)
        {
        }

        public override void Execute(object parameter)
        {
            _ruler.MoveLeft();
        }
    }
}
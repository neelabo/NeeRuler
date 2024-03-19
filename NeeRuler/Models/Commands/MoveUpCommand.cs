namespace NeeRuler.Models
{
    public class MoveUpCommand : ReadingRulerCommand
    {
        public MoveUpCommand(ReadingRuler ruler) : base(ruler)
        {
        }

        public override void Execute(object parameter)
        {
            _ruler.MoveUp();
        }
    }
}
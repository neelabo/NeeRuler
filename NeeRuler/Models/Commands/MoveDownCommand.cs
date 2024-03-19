namespace NeeRuler.Models
{
    public class MoveDownCommand : ReadingRulerCommand
    {
        public MoveDownCommand(ReadingRuler ruler) : base(ruler)
        {
        }

        public override void Execute(object parameter)
        {
            _ruler.MoveDown();
        }
    }
}
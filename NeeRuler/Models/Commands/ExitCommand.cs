namespace NeeRuler.Models
{
    public class ExitCommand : ReadingRulerCommand
    {
        public ExitCommand(ReadingRuler ruler) : base(ruler)
        {
        }

        public override void Execute(object parameter)
        {
            _ruler.Exit();
        }
    }
}
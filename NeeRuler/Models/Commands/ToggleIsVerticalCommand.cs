namespace NeeRuler.Models
{
    public class ToggleIsVerticalCommand : ReadingRulerCommand
    {
        public ToggleIsVerticalCommand(ReadingRuler ruler) : base(ruler)
        {
        }

        public override void Execute(object parameter)
        {
            _ruler.ToggleIsVertical();
        }
    }
}
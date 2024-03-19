namespace NeeRuler.Models
{
    public class ToggleIsAutoStrideCommand : ReadingRulerCommand
    {
        public ToggleIsAutoStrideCommand(ReadingRuler ruler) : base(ruler)
        {
        }

        public override void Execute(object parameter)
        {
            _ruler.ToggleIsAutoStride();
        }
    }
}
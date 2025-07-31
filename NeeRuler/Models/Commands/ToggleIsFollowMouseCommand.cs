namespace NeeRuler.Models
{
    public class ToggleIsFollowMouseCommand : ReadingRulerCommand
    {
        public ToggleIsFollowMouseCommand(ReadingRuler ruler) : base(ruler)
        {
        }

        public override void Execute(object parameter)
        {
            _ruler.ToggleIsFollowMouse();
        }
    }
}
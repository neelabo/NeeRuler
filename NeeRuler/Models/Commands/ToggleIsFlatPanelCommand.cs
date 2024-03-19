namespace NeeRuler.Models
{
    public class ToggleIsFlatPanelCommand : ReadingRulerCommand
    {
        public ToggleIsFlatPanelCommand(ReadingRuler ruler) : base(ruler)
        {
        }

        public override void Execute(object parameter)
        {
            _ruler.ToggleIsFlatPanel();
        }
    }
}
namespace NeeRuler.Models
{
    public class RestoreLocationCommand : ReadingRulerCommand
    {
        public RestoreLocationCommand(ReadingRuler ruler) : base(ruler)
        {
        }

        public override void Execute(object parameter)
        {
            _ruler.RestoreLocation();
        }
    }
}
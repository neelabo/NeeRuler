namespace NeeRuler.Models
{
    public class StoreLocationCommand : ReadingRulerCommand
    {
        public StoreLocationCommand(ReadingRuler ruler) : base(ruler)
        {
        }

        public override void Execute(object parameter)
        {
            _ruler.StoreLocation();
        }
    }
}
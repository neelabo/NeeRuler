namespace NeeRuler.Models
{
    public class OpenProfilesFolderCommand : ReadingRulerCommand
    {
        public OpenProfilesFolderCommand(ReadingRuler ruler) : base(ruler)
        {
        }

        public override void Execute(object parameter)
        {
            _ruler.OpenProfilesFolder();
        }
    }
}
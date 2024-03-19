using System;

namespace NeeRuler.Models
{
    public class ProfileCommand : ReadingRulerCommand
    {
        private readonly string _id;

        public ProfileCommand(ReadingRuler ruler, string id) : base(ruler)
        {
            _id = id;
        }

        public override string Name => base.Name + "-" + _id;

        public override void Execute(object parameter)
        {
            _ruler.ImportProfile(_id);
        }
    }
}
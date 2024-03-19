using System;
using System.Windows.Input;

namespace NeeRuler.Models
{
    public abstract class ReadingRulerCommand : ICommand
    {
        protected ReadingRuler _ruler;

        protected ReadingRulerCommand(ReadingRuler ruler)
        {
            _ruler = ruler;
        }


        public event EventHandler? CanExecuteChanged;


        public virtual string Name
        {
            get => CommandNameConverter.Create(this.GetType());
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        public abstract void Execute(object parameter);
    }
}
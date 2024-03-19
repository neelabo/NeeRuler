using System;
using System.Windows.Input;

namespace NeeLaboratory.Windows.Input
{
    public class NopCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            return false;
        }

        public void Execute(object parameter)
        {
        }
    }
}
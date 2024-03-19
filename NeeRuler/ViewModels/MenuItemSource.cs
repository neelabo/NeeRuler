using System.Windows.Input;

namespace NeeRuler.ViewModels
{
    public class MenuItemSource
    {
        public MenuItemSource() : this("", null)
        {
        }

        public MenuItemSource(string header) : this(header, null)
        {
        }

        public MenuItemSource(string header, ICommand? command)
        {
            Header = header;
            Command = command;
        }

        public string Header { get; set; }
        public ICommand? Command { get; set; }
        public bool IsEnabled { get; set; } = true;
    }
}
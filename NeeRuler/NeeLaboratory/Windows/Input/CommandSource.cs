using System.Collections.Generic;
using System.Windows.Input;

namespace NeeLaboratory.Windows.Input
{
    public class CommandSource
    {

        public CommandSource(ICommand command) : this(command, new List<InputGesture>())
        {
        }

        public CommandSource(ICommand command, List<InputGesture> inputGestures)
        {
            Command = command;
            InputGestures = inputGestures;
        }

        public ICommand Command { get; }
        public List<InputGesture> InputGestures { get; set; }
    }
}
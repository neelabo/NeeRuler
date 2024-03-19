using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace NeeLaboratory.Windows.Input
{
    public class CommandMap
    {
        private static readonly char[] _tokenSeparator = new char[] { ',' };
        private readonly Dictionary<string, CommandSource> _commands;
        private readonly CommandSource _nop = new CommandSource(new NopCommand());

        public CommandMap()
        {
            _commands = new Dictionary<string, CommandSource>();
        }

        public Dictionary<string, CommandSource> Commands => _commands;


        public CommandSource GetCommand(string key)
        {
            if (_commands.TryGetValue(key, out var command))
                return command;
            else
                return _nop;
        }


        public void ExecuteCommand(string key, object? value)
        {
            var command = _commands[key].Command;
            if (command.CanExecute(value))
            {
                command.Execute(value);
            }
        }

        public void AddCommand(string key, ICommand command)
        {
            _commands.Add(key, new CommandSource(command));
        }

        public void AddCommand(string key, ICommand command, string gesture)
        {
            _commands.Add(key, new CommandSource(command, StringToInputGesture(gesture)));
        }

        public void ClearInputGesture()
        {
            foreach (var command in _commands.Values)
            {
                command.InputGestures = new List<InputGesture>();
            }
        }

        public void SetInputGesture(string key, string gestures)
        {
            if (_commands.TryGetValue(key, out var commandSource))
            {
                commandSource.InputGestures = StringToInputGesture(gestures);
            }
            else
            {
                Debug.WriteLine($"Cannot find command: {key}");
            }
        }

        private List<InputGesture> StringToInputGesture(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return new List<InputGesture>();

            return text
                .Split(_tokenSeparator, StringSplitOptions.RemoveEmptyEntries)
                .Select(e => ConvertToInputGesture(e.Trim()))
                .ToList();

            InputGesture ConvertToInputGesture(string s)
            {
                var keyGestureConverter = new KeyExGestureConverter();
                return (InputGesture)keyGestureConverter.ConvertFromString(s);
            }
        }

        public void ClearInputBinding(FrameworkElement element)
        {
            element.InputBindings.Clear();
        }

        public void AddInputBinding(FrameworkElement element)
        {
            foreach (var command in _commands.Values)
            {
                AddInputBinding(element, command.Command, command.InputGestures);
            }
        }

        public void AddInputBinding(FrameworkElement element, string key)
        {
            var command = _commands[key];
            AddInputBinding(element, command.Command, command.InputGestures);
        }

        public static void AddInputBinding(FrameworkElement element, ICommand command, IEnumerable<InputGesture> inputGestures)
        {
            foreach (var inputGesture in inputGestures)
            {
                element.InputBindings.Add(new InputBinding(command, inputGesture));
            }
        }
    }
}
using NeeLaboratory;
using NeeLaboratory.Text;
using NeeRuler.ViewModels;
using System;
using System.Windows.Input;

namespace NeeRuler.Models
{
    public static class CommandNameConverter
    {
        public static string Create<T>()
            where T : ICommand
        {
            return Create(typeof(T));
        }

        public static string Create(Type type)
        {
            return TrimPostfix(type.Name);
        }

        private static string TrimPostfix(string name)
        {
            return name.TrimPostfix("Command");
        }
    }
}
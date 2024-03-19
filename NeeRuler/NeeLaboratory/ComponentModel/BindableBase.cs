// from http://sourcechord.hatenablog.com/entry/20130303/1362315081
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NeeLaboratory.ComponentModel
{
    public abstract class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public IDisposable SubscribePropertyChanged(PropertyChangedEventHandler handler)
        {
            PropertyChanged += handler;
            return new AnonymousDisposable(() => PropertyChanged -= handler);
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

            storage = value;
            this.RaisePropertyChanged(propertyName);
            return true;
        }

        protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void ResetPropertyChanged()
        {
            this.PropertyChanged = null;
        }

        protected PropertyChangedDependency CreatePropertyChangedDependency(INotifyPropertyChanged source)
        {
            return new PropertyChangedDependency(source, RaisePropertyChanged);
        }

#if false
        private void RaisePropertyChanged(object sender, PropertyChangedEventArgs e, RelayPropertyMap map)
        {
            if (string.IsNullOrEmpty(e.PropertyName))
            {
                foreach (var property in map.Values.Where(x => x.RelayProperties is not null).SelectMany(x => x.RelayProperties).Distinct())
                {
                    RaisePropertyChanged(property);
                }
                foreach (var callback in map.Values.Where(x => x.Callback is not null).Select(x => x.Callback).Distinct())
                {
                    callback?.Invoke(sender, e);
                }
            }
            else if (map.TryGetValue(e.PropertyName, out var description))
            {
                if (description.RelayProperties is not null)
                {
                    foreach (var property in description.RelayProperties)
                    {
                        RaisePropertyChanged(property);
                    }
                }
                if (description.Callback is not null)
                {
                    description.Callback(sender, e);
                }
            }
        }
#endif
    }

}

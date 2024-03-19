using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace NeeLaboratory.ComponentModel
{
    public class PropertyChangedDependency : IDisposable
    {
        private readonly INotifyPropertyChanged _source;
        private readonly Action<string>? _raisePropertyChanged;
        private readonly Dictionary<string, PropertyChangedDescription> _map = new();
        private bool _disposedValue;


        public PropertyChangedDependency(INotifyPropertyChanged source) : this(source, null)
        {
        }

        public PropertyChangedDependency(INotifyPropertyChanged source, Action<string>? raisePropertyChanged)
        {
            _source = source; ;
            _raisePropertyChanged = raisePropertyChanged;

            _source.PropertyChanged += Source_PropertyChanged;
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _source.PropertyChanged -= Source_PropertyChanged;
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


        public void Add(string sourcePropertyName)
        {
            Debug.Assert(_raisePropertyChanged != null, "RaisePropertyChanged method is required");
            _map.Add(sourcePropertyName, new PropertyChangedDescription(new[] { sourcePropertyName }));
        }

        public void Add(string sourcePropertyName, string raisePropertyName)
        {
            Debug.Assert(_raisePropertyChanged != null, "RaisePropertyChanged method is required");
            _map.Add(sourcePropertyName, new PropertyChangedDescription(new[] { raisePropertyName }));
        }

        public void Add(string sourcePropertyName, string raisePropertyName, params string[] raisePropertyNames)
        {
            Debug.Assert(_raisePropertyChanged != null, "RaisePropertyChanged method is required");
            _map.Add(sourcePropertyName, new PropertyChangedDescription(raisePropertyNames.Prepend(raisePropertyName).ToArray()));
        }

        public void Add(string sourcePropertyName, PropertyChangedEventHandler callback)
        {
            _map.Add(sourcePropertyName, new PropertyChangedDescription(callback));
        }

        public void Add(string sourcePropertyName, string raisePropertyName, PropertyChangedEventHandler callback)
        {
            Debug.Assert(_raisePropertyChanged != null, "RaisePropertyChanged method is required");
            _map.Add(sourcePropertyName, new PropertyChangedDescription(new[] { raisePropertyName }, callback));
        }


        private void Source_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName))
            {
                foreach (var property in _map.Values.Where(x => x.RelayProperties is not null).SelectMany(x => x.RelayProperties).Distinct())
                {
                    _raisePropertyChanged?.Invoke(property);
                }
                foreach (var callback in _map.Values.Where(x => x.Callback is not null).Select(x => x.Callback).Distinct())
                {
                    callback?.Invoke(sender, e);
                }
            }
            else if (_map.TryGetValue(e.PropertyName, out var description))
            {
                if (description.RelayProperties is not null)
                {
                    foreach (var property in description.RelayProperties)
                    {
                        _raisePropertyChanged?.Invoke(property);
                    }
                }
                if (description.Callback is not null)
                {
                    description.Callback(sender, e);
                }
            }
        }

    }
}

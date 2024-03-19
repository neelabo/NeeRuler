using System;
using System.Collections.Generic;
using System.Linq;

namespace NeeLaboratory.ComponentModel
{
    public class DisposableCollection : List<IDisposable>, IDisposable
    {
        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    foreach (var disposable in this.Reverse<IDisposable>())
                    {
                        disposable.Dispose();
                    }
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


        public void Add(Action action)
        {
            Add(new AnonymousDisposable(action));
        }
    }
}
using System;

namespace NeeLaboratory.ComponentModel
{
    public class AnonymousDisposable : IDisposable
    {
        private Action _disposeAction;
        private bool _disposedValue;

        public AnonymousDisposable(Action disposeAction)
        {
            _disposeAction = disposeAction;
        }

        public static IDisposable Create(Action action)
        {
            return new AnonymousDisposable(action);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _disposeAction();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }


}
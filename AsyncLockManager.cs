using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kodak.RISr2.Lib.util
{
    public class AsyncLockManager : IDisposable
    {
        private readonly SemaphoreSlim _semaphore= new SemaphoreSlim(1, 1);
        private bool _disposed = false;

        /// <summary>
        /// excec async method atomic
        /// </summary>
        /// <param name="workerMethod"></param>
        /// <returns></returns>
        public async Task ExcecuteLockedAsync(Func<Task> workerMethod)
        {
            checkDisposed();
            await _semaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                await workerMethod().ConfigureAwait(false);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        /// <summary>
        /// get async lock object
        /// </summary>
        /// <returns></returns>
        public async Task<IDisposable> GetLockAsync()
        {
            checkDisposed();
            await _semaphore.WaitAsync().ConfigureAwait(false);
            return new Finalizer(_semaphore);
        }
        /// <summary>
        /// get async lock object
        /// </summary>
        /// <param name="onDispose">delegate will be called from Dispose() method</param>
        /// <returns></returns>
        public async Task<IDisposable> GetLockAsync(Action onDispose)
        {
            checkDisposed();
            await _semaphore.WaitAsync().ConfigureAwait(false);
            return new Finalizer(_semaphore, onDispose);
        }

        #region IDisposable implementation
        public void Dispose()
        {
            _semaphore.Dispose();
            _disposed = true;
        }
        #endregion

        private void checkDisposed()
        {
            if (_disposed)
                throw new Exception($"The object {nameof(AsyncLockManager)} was disposed");
        }

        class Finalizer : IDisposable
        {
            private SemaphoreSlim _semaphore;
            private Action _onDispose;
            public Finalizer(SemaphoreSlim semaphore) 
                : this(semaphore, () => { })
            {
                
            }
            public Finalizer(SemaphoreSlim semaphore,Action onDispose)
            {
                _semaphore = semaphore;
                _onDispose = onDispose;
            }
            void IDisposable.Dispose()
            {
                _onDispose();
                _semaphore.Release();
            }
        }

    }
}

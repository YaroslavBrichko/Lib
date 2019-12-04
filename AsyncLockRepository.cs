using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Kodak.RISr2.Lib.util
{
    /// <summary>
    /// named lock objects
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class AsyncLockRepository<TKey>
    {
        private ConcurrentDictionary<TKey, WeakReference<AsyncLockManager>> _repository;

        public AsyncLockRepository()
        {
            _repository = new ConcurrentDictionary<TKey, WeakReference<AsyncLockManager>>();
        }

        /// <summary>
        /// get lock object by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<IDisposable> TryGetLockAsync(TKey key)
        {
            AsyncLockManager theManager = null;
            WeakReference<AsyncLockManager> @ref =
                _repository.GetOrAdd(key, (k) =>
                 {
                     var manager = new AsyncLockManager();
                     return new WeakReference<AsyncLockManager>(manager);
                 });
 
            @ref.TryGetTarget(out theManager);
            if(theManager==null)
            {
                _repository.TryRemove(key, out @ref);
                return TryGetLockAsync(key);
            }
            return theManager.GetLockAsync();
        }
    }
}

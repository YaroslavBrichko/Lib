using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kodak.RISr2.Lib.util
{
    public class AsyncLockOneTimeActionManager
    {
        private int _locker;
        /// <summary>
        /// every call coming while the previous one executing won't be queued and skipped
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task OneTimeActionAsync(Func<Task> action)
        {
            if (0 == Interlocked.Exchange(ref _locker, 1))
            {
                try
                {
                    await action();
                }
                finally
                {
                    Interlocked.Exchange(ref _locker, 0);
                }
            }
            else
                await Task.CompletedTask;
        }
    }

    public class AsyncLockOneTimeActionManager<T>
    {
        private int _locker;
        /// <summary>
        /// every call coming while the previous one executing won't be queued and skipped
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task<T> OneTimeActionAsync(Func<Task<T>> action)
        {
            if (0 == Interlocked.Exchange(ref _locker, 1))
            {
                try
                {
                   return await action();
                }
                finally
                {
                    Interlocked.Exchange(ref _locker, 0);
                }
            }
            else
                return await Task.FromResult(default(T));
        }
    }

    public class LockOneTimeActionManager<T>
    {
        private int _locker;
        /// <summary>
        /// every call coming while the previous one executing won't be queued and skipped
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public T OneTimeAction(Func<T> action)
        {
            if (0 == Interlocked.Exchange(ref _locker, 1))
            {
                try
                {
                   return  action();
                }
                finally
                {
                    Interlocked.Exchange(ref _locker, 0);
                }
            }
            return default(T);
        }
    }
}

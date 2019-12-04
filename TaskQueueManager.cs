using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Kodak.RISr2.Lib.TaskHelpers
{
    public interface ITaskHandler<TKey, TValue>
    {
        void HandleTask(TKey key, TValue value);
    }

    /// <summary>
    /// create list of deferred tasks and handle them consequently
    /// </summary>
    /// <typeparam name="TKey">unique name of task</typeparam>
    /// <typeparam name="TValue">data to process</typeparam>
    public class TaskQueueManager<TKey, TValue> : IDisposable
    {
        private CancellationTokenSource _theToken;
        private ConcurrentQueue<DataStruct> _tasksQueue;
        private ConcurrentDictionary<TKey, TValue> _processedDataController;
        private ManualResetEventSlim _taskSignal;
        private Task _backgroundTask = null;

        private ITaskHandler<TKey, TValue> _handler;

        #region .ctor
        public TaskQueueManager(bool manualStart, ITaskHandler<TKey, TValue> handler)
        {
            _theToken = new CancellationTokenSource();
            _tasksQueue = new ConcurrentQueue<DataStruct>();
            _processedDataController = new ConcurrentDictionary<TKey, TValue>();
            _taskSignal = new ManualResetEventSlim(false);

            _handler = handler;

            if (!manualStart)
            {
                Run();
            }
        }
        #endregion

        #region public methods implementation
        public bool TryGetData(TKey name, out TValue data)
        {
            return _processedDataController.TryGetValue(name, out data);
        }

        public void PushTask(TKey name, TValue data)
        {
            if (_processedDataController.TryAdd(name, data))
            {
                _tasksQueue.Enqueue(new DataStruct(name, data));
                _taskSignal.Set();
            }
        }

        public Task Run()
        {
            return _backgroundTask ?? Task.Factory.StartNew(() =>
            {
                do
                {
                    DataStruct taskData;
                    while (_tasksQueue.TryDequeue(out taskData))
                    {
                        _handler.HandleTask(taskData.Key, taskData.Data);
                        _taskSignal.Reset();
                    }
                    try
                    {
                        _taskSignal.Wait(_theToken.Token);
                    }
                    catch (OperationCanceledException)
                    {

                    }
                    catch
                    {
                        throw;
                    }
                }
                while (!_theToken.IsCancellationRequested);

            }, _theToken.Token);
        }

        public void Cancel()
        {
            _processedDataController.Clear();
            _theToken.Cancel();
            _backgroundTask = null;
        }
        #endregion

        #region IDisposable implementation
        void IDisposable.Dispose()
        {
            Cancel();
        }
        #endregion

        class DataStruct
        {
            public TValue Data { get; }
            public TKey Key { get; }

            public DataStruct(TKey key, TValue data)
            {
                Data = data;
                Key = key;
            }

        }
    }

}

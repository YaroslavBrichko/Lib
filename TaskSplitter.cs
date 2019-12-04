using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kodak.RISr2.Lib
{
    /// <summary>
    /// class helps to handle large collection in parallel in multiple chunks
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TaskSplitter<T>
    {
        private int _maxChunkLength;
        private IReadOnlyCollection<T> _collection;
        protected readonly int _finalCountOfTasks;

        #region .ctor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="maxChunkLength"></param>
        public TaskSplitter(IReadOnlyCollection<T> collection, int maxChunkLength)
        {
            _maxChunkLength = maxChunkLength;
            _collection = collection;
            int len = collection.Count / _maxChunkLength;
            _finalCountOfTasks = collection.Count % _maxChunkLength > 0 ? len + 1 : len;
        }
        #endregion

        #region Splitt implementation
        public void Split(Func<IEnumerable<T>, Task> chunckHandler)
        {
            var tasks = execute(chunckHandler);
            Task.WaitAll(tasks);
        }

        public async Task SplitAsync(Func<IEnumerable<T>, Task> chunckHandler)
        {
            var tasks = execute(chunckHandler);
            await Task.WhenAll(tasks);
        }

        public void Split(Action<IEnumerable<T>> chunkHandler)
        {
            var tasks = new Task[_finalCountOfTasks];

            for (int i = 0; i < _finalCountOfTasks; i++)
            {
                var chunk = GetNextChunk(i);
                var task = Task.Factory.StartNew(() => chunkHandler(chunk));
                tasks[i] = task;
            }

            Task.WaitAll(tasks);

        }

        public async Task<Result[]> Split<Result>(Func<IEnumerable<T>, Task<Result>> chunkHandler)
        {
            var tasks = new Task<Result>[_finalCountOfTasks];

            for (int i = 0; i < _finalCountOfTasks; i++)
            {
                var chunk = GetNextChunk(i);
                var task = chunkHandler(chunk);
                tasks[i] = task;
            }

            return await Task.WhenAll(tasks);

        }
        #endregion

        private Task[] execute(Func<IEnumerable<T>, Task> chunckHandler)
        {
            var tasks = new Task[_finalCountOfTasks];

            for (int i = 0; i < _finalCountOfTasks; i++)
            {
                var chunk = GetNextChunk(i);
                var task = chunckHandler(chunk);
                tasks[i] = task;
            }
            return tasks;
        }

        private IEnumerable<T> GetNextChunk(int i)
        {
            return _collection.Skip(i * _maxChunkLength).Take(_maxChunkLength);
        }
    }
}

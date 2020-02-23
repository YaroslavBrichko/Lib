using System;
using System.Linq;
using System.Threading.Tasks;

namespace Kodak.RISr2.Lib.util
{
    public static class EventHandlerAsyncHelper
    {
        /// <summary>
        /// raice event handlers sequentially
        /// </summary>
        /// <typeparam name="TEventArgs"></typeparam>
        /// <param name="event"></param>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Task InvokeAsync<TEventArgs>(this Func<object, TEventArgs, Task> @event, object sender, TEventArgs args) where TEventArgs: EventArgs
        {
            return InvokeAsync<object,TEventArgs>(@event, sender, args);
        }
        /// <summary>
        /// raice event handlers sequentially
        /// </summary>
        /// <typeparam name="TArgs"></typeparam>
        /// <param name="event"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async static Task InvokeAsync<TArgs>(this Func<TArgs, Task> @event, TArgs args) 
        {
            if (@event == null)
            {
                await Task.CompletedTask;
            }
            var evtHandler = @event;
            var handlers = evtHandler.GetInvocationList()
                          .Cast<Func<TArgs, Task>>();

            foreach (Func<TArgs, Task> handler in handlers)
            {
                await handler(args);
            }
        }

        /// <summary>
        /// raice event handlers sequentially
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TEventArgs"></typeparam>
        /// <param name="event"></param>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task InvokeAsync<TSource, TEventArgs>(this Func<TSource, TEventArgs, Task> @event, TSource sender, TEventArgs args)
            where TSource : class
            where TEventArgs : EventArgs
        {
            if (@event == null)
            {
                await Task.CompletedTask;
                return;
            }
            var evtHandler = @event;
            var handlers = evtHandler.GetInvocationList()
                          .Cast<Func<TSource, TEventArgs, Task>>();

            foreach(Func<TSource, TEventArgs, Task> handler in handlers)
            {
                await handler(sender, args);
            }
        }

        public static async Task<TResult[]> InvokeAsync<TSource, TEventArgs, TResult>(this Func<TSource, TEventArgs, Task> @event, TSource sender, TEventArgs args)
            where TEventArgs : EventArgs
        {
            if (@event == null)
                return new TResult[] { };
            var evtHandler = @event;
            var delegates = evtHandler.GetInvocationList();
            TResult[] resultTasks = new TResult[delegates.Length];
            var handlers = evtHandler.GetInvocationList()
                          .Cast<Func<TSource, TEventArgs, Task<TResult>>>();
            int i = 0;
            foreach(Func<TSource, TEventArgs, Task<TResult>> handler in handlers)
            {
                resultTasks[i] = await handler(sender, args);
                i++;
            }
            return resultTasks;
        }

        public static async Task<TResult[]> InvokeAsync<TParameter, TResult>(this Func<TParameter, Task> @event, TParameter args)
        {
            if (@event == null)
                return new TResult[] { };
            var evtHandler = @event;
            var delegates = evtHandler.GetInvocationList();
            TResult[] resultTasks = new TResult[delegates.Length];
            var handlers = evtHandler.GetInvocationList()
                          .Cast<Func<TParameter, Task<TResult>>>();
            int i = 0;
            foreach(Func<TParameter,Task<TResult>> handler in handlers)
            {
                resultTasks[i] = await handler(args);
                i++;
            }
            return resultTasks;
        }

        public static void DisposeEvents<TEventArgs>(ref Func<object, TEventArgs, Task> @event) where TEventArgs : EventArgs
        {
            DisposeEvents<object, TEventArgs>(ref @event);
        }

        public static void DisposeEvents<TResult, TSource, TEventArgs>(ref Func<TSource, TEventArgs, Task<TResult>> @event)
            where TEventArgs : EventArgs
        {
            if (@event != null)
            {
                var evtHandler = @event;
                var handlers = evtHandler.GetInvocationList().Cast<Func<TSource, TEventArgs, Task<TResult>>>();
                foreach (var handler in handlers)
                {
                    evtHandler -= handler;
                }
                @event = evtHandler;
            }
        }

        public static void DisposeEvents<TSource, TEventArgs>(ref Func<TSource, TEventArgs, Task> @event)
            where TEventArgs : EventArgs
        {
            if (@event != null)
            {
                var evtHandler = @event;
                var handlers = evtHandler.GetInvocationList().Cast<Func<TSource, TEventArgs, Task>>();
                foreach (var handler in handlers)
                {
                    evtHandler -= handler;
                }
                @event = evtHandler;
            }
        }
    }
}

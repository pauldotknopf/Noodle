using System;
using System.Threading;

namespace Noodle.Engine
{
    /// <summary>
    /// Performs work asynchronously.
    /// </summary>
    public class AsyncWorker : IWorker
    {
        int _executingWorkItems = 0;

        /// <summary>Testability seam for the async worker.</summary>
        public Func<WaitCallback, bool> QueueUserWorkItem = ThreadPool.QueueUserWorkItem;

        #region IWorker Members

        /// <summary>Gets the number of executing work actions.</summary>
        public int ExecutingWorkItems
        {
            get { return _executingWorkItems; }
        }

        /// <summary>Starts the execution of the specified work.</summary>
        /// <param name="action">The method to execute.</param>
        public virtual void DoWork(Action action)
        {
            Interlocked.Increment(ref _executingWorkItems);
            QueueUserWorkItem(delegate
            {
                try
                {
                    action();
                }
                finally
                {
                    Interlocked.Decrement(ref _executingWorkItems);
                }
            });
        }

        /// <summary>Starts the execution of the specified work with error handling.</summary>
        /// <param name="action">The method to execute.</param>
        /// <param name="onError">Action to execute when an error occurs.</param>
        /// <typeparam name="T">The type of exceptions to handle.</typeparam>
        public virtual void DoWork<T>(Action action, Action<T> onError) where T : Exception
        {
            Interlocked.Increment(ref _executingWorkItems);
            QueueUserWorkItem(delegate
            {
                try
                {
                    action();
                }
                catch (T ex)
                {
                    onError(ex);
                }
                finally
                {
                    Interlocked.Decrement(ref _executingWorkItems);
                }
            });
        }

        #endregion
    }
}

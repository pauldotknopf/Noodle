using System;
using System.Threading;

namespace Noodle.Tests
{
    public static class RetryUtility
    {
        public static void RetryAction(Action action, int numRetries, int retryTimeout, Action<Exception> onError = null)
        {
            if (action == null)
                throw new ArgumentNullException("action"); // slightly safer...

            do
            {
                try { action(); return; }
                catch(Exception ex)
                {
                    if (onError != null)
                    {
                        onError(ex);
                    }
                    if (numRetries <= 0) throw;  // improved to avoid silent failure
                    else Thread.Sleep(retryTimeout);
                }
            } while (numRetries-- > 0);
        }
    }
}

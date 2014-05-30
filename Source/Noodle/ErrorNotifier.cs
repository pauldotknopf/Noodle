using System;

namespace Noodle
{
    /// <summary>
    /// Internal services notifiy the system of errors through this interface
    /// </summary>
    public class ErrorNotifier : IErrorNotifier
    {
        #region IErrorNotifier Members

        /// <summary>
        /// Notify the system of an exception
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        public void Notify(string message, Exception ex)
        {
            if (ErrorOccured != null)
                ErrorOccured(this, new ErrorNotifierEventArgs(message,  ex));
        }

        /// <summary>
        /// Raised when an error occurs
        /// </summary>
        public event EventHandler<ErrorNotifierEventArgs> ErrorOccured;

        #endregion
    }
}

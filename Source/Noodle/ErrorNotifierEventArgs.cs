using System;

namespace Noodle
{
    /// <summary>
    /// The events args used by IErrorNotifier
    /// </summary>
    public class ErrorNotifierEventArgs : EventArgs
    {
        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorNotifierEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        public ErrorNotifierEventArgs(string message, Exception ex)
        {
            Message = message;
            Exception = ex;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The message
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// The exception
        /// </summary>
        public Exception Exception { get; private set; }

        #endregion
    }
}

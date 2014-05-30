using System;

namespace Noodle
{
    /// <summary>
    /// Internal services notifiy the system of errors through this interface
    /// </summary>
    public interface IErrorNotifier
    {
        /// <summary>
        /// Notify the system of an exception
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        void Notify(string message, Exception ex);
        
        /// <summary>
        /// Raised when an error occurs
        /// </summary>
        event EventHandler<ErrorNotifierEventArgs> ErrorOccured;
    }
}

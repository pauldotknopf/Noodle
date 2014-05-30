using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Noodle.Engine;

namespace Noodle.Logging
{
    /// <summary>
    /// This classes listens to error notifications and logs them
    /// </summary>
    public class ErrorNotifierLogger : IStartupTask, IDisposable
    {
        #region Fields

        private readonly IErrorNotifier _errorNotifier;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorNotifierLogger"/> class.
        /// </summary>
        /// <param name="errorNotifier">The error notifier.</param>
        /// <param name="logger">The logger.</param>
        public ErrorNotifierLogger(IErrorNotifier errorNotifier, ILogger logger)
        {
            _errorNotifier = errorNotifier;
            _logger = logger;
        }

        #endregion

        #region IStartupTask

        /// <summary>
        /// Excute is run once on startup of the application
        /// </summary>
        public void Execute()
        {
            _errorNotifier.ErrorOccured += OnErrorOccured;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _errorNotifier.ErrorOccured -= OnErrorOccured;
        }

        /// <summary>
        /// The order at which the startup task will run. Smaller numbers run first.
        /// </summary>
        public int Order
        {
            get { return int.MaxValue; }
        }

        #endregion

        /// <summary>
        /// Raised when someone calls IErrorNotifier.Notify
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ErrorNotifierEventArgs"/> instance containing the event data.</param>
        private void OnErrorOccured(object sender, ErrorNotifierEventArgs e)
        {
            if (e.Exception != null && !LogStore.IsErrorLoggable(e.Exception)) return;

            try
            {
                if (e.Exception is LogException)
                {
                    _logger.InsertLog((e.Exception as LogException).LogLevel,
                                      !string.IsNullOrEmpty(e.Message)
                                          ? e.Message + "-" + (e.Exception as LogException).ShortMessage
                                          : (e.Exception as LogException).ShortMessage,
                                      (e.Exception as LogException).FullMessage,
                                      e.Exception);
                }
                else
                {
                    _logger.Error(e.Message, e.Exception);
                }
            }
            catch (Exception ex)
            {
                new Logger<ErrorNotifierLogger>().Error("There was a problem logging an exception from IErrorNotifier. " + ex.Message);
            }
        }
    }
}

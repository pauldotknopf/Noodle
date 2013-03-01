using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Noodle.Engine;
using Noodle.Web;

namespace Noodle.Logging
{
    /// <summary>
    /// This classes listens to error notifications and logs them
    /// </summary>
    public class ErrorNotifierLogger : IStartupTask, IDisposable
    {
        private readonly IErrorNotifier _errorNotifier;
        private readonly ILogger _logger;

        /// <summary>
        /// Ctor
        /// </summary>
        public ErrorNotifierLogger(IErrorNotifier errorNotifier, ILogger logger)
        {
            _errorNotifier = errorNotifier;
            _logger = logger;
        }

        public void Execute()
        {
            _errorNotifier.ErrorOccured += ErrorNotifierOnErrorOccured;
        }

        public void Dispose()
        {
            _errorNotifier.ErrorOccured -= ErrorNotifierOnErrorOccured;
        }

        public int Order
        {
            get { return int.MaxValue; }
        }

        private void ErrorNotifierOnErrorOccured(object sender, NoodleEventArgs<Exception> errorEventArgs)
        {
            if(errorEventArgs.Item == null) return;
            if (!LogStore.IsErrorLoggable(errorEventArgs.Item)) return;

            try
            {
                if (errorEventArgs.Item is LogException)
                {
                    _logger.InsertLog((errorEventArgs.Item as LogException).LogLevel,
                                      (errorEventArgs.Item as LogException).ShortMessage,
                                      (errorEventArgs.Item as LogException).FullMessage,
                                      errorEventArgs.Item);
                }else
                {
                    _logger.Error("IErrorNotifier", errorEventArgs.Item);
                }
            }catch(Exception ex)
            {
                new Logger<ErrorNotifierLogger>().Error("There was a problem logging an exception from IErrorNotifier. " + ex.Message);
            }
        }
    }
}

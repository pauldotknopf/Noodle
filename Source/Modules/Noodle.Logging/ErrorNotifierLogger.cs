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
            get { throw new NotImplementedException(); }
        }

        private void ErrorNotifierOnErrorOccured(object sender, ErrorEventArgs errorEventArgs)
        {
            if(errorEventArgs.Error == null) return;
            if (!LogStore.IsErrorLoggable(errorEventArgs.Error)) return;

            try
            {
                if(errorEventArgs.Error is LogException)
                {
                    _logger.InsertLog((errorEventArgs.Error as LogException).LogLevel, 
                                      (errorEventArgs.Error as LogException).ShortMessage,
                                      (errorEventArgs.Error as LogException).FullMessage, 
                                      errorEventArgs.Error);
                }else
                {
                    _logger.Error("IErrorNotifier", errorEventArgs.Error);
                }
            }catch(Exception ex)
            {
                new Logger<ErrorNotifierLogger>().Error("There was a problem logging an exception from IErrorNotifier. " + ex.Message);
            }
        }
    }
}

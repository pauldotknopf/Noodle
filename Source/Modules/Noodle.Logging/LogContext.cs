using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noodle.Logging
{
    /// <summary>
    /// Quick access to some logging services
    /// </summary>
    public class LogContext
    {
        /// <summary>
        /// Get the logger
        /// </summary>
        public static ILogger Logger { get { return EngineContext.Resolve<ILogger>(); } }
    }
}

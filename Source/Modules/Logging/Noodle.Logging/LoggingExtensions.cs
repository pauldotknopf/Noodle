using System;

namespace Noodle.Logging
{
    public static class LoggingExtensions
    {
        public static void Debug(this ILogger logger, string message, Exception exception = null, string user = null)
        {
            FilteredLog(logger, LogLevel.Debug, message, exception, user);
        }

        public static void Information(this ILogger logger, string message, Exception exception = null, string user = null)
        {
            FilteredLog(logger, LogLevel.Information, message, exception, user);
        }

        public static void Warning(this ILogger logger, string message, Exception exception = null, string user = null)
        {
            FilteredLog(logger, LogLevel.Warning, message, exception, user);
        }

        public static void Error(this ILogger logger, string message, Exception exception = null, string user = null)
        {
            FilteredLog(logger, LogLevel.Error, message, exception, user);
        }

        public static void Fatal(this ILogger logger, string message, Exception exception = null, string user = null)
        {
            FilteredLog(logger, LogLevel.Fatal, message, exception, user);
        }

        private static void FilteredLog(ILogger logger, LogLevel level, string message, Exception exception = null, string user = null)
        {
            logger.InsertLog(level, message, string.Empty, exception, user);
        }
    }
}

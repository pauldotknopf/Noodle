using System;

namespace Noodle.Logging
{
    /// <summary>
    /// This is an exception passed to "GetCustomData" if there is a log entered with no exception (informational).
    /// This contains information about the information being logged so that GetCustomData actions can analyze the information if they need to.
    /// </summary>
    public class LogException : Exception
    {
        public LogLevel LogLevel { get; protected set; }
        public string ShortMessage { get; protected set; }
        public string FullMessage { get; protected set; }

        public LogException(LogLevel logLevel, string shortMessage, string fullMessage)
        {
            LogLevel = logLevel;
            ShortMessage = shortMessage;
            FullMessage = fullMessage;
        }
    }
}

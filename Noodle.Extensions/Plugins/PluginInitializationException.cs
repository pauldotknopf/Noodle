using System;

namespace Noodle.Extensions.Plugins
{
    public class PluginInitializationException : Exception
    {
        public PluginInitializationException(string message, Exception[] innerExceptions)
            : base(message, innerExceptions[0])
        {
            InnerExceptions = innerExceptions;
        }

        public Exception[] InnerExceptions { get; set; }
    }
}

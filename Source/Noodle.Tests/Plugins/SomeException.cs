using System;

namespace Noodle.Tests.Plugins
{
    public class SomeException : Exception
    {
        public SomeException(string message)
            : base(message)
        {
        }
    }
}

using System;

namespace Noodle
{
    public class ErrorEventArgs : EventArgs
    {
        public Exception Error { get; set; }
    }
}

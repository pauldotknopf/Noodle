using System;

namespace Noodle
{
    public class NoodleEventArgs<T> : EventArgs
    {
        public NoodleEventArgs(T item)
        {
            Item = item;
        }

        public T Item { get; protected set; }
    }
}

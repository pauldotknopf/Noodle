using System.Collections.Generic;
using System.Timers;

namespace Noodle.Collections
{
    /// <summary>
    /// An item that is capable of removing itself from a list at a specified interval
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExpiringItem<T>
    {
        private readonly T _item;
        readonly Timer _timer;
        readonly List<ExpiringItem<T>> _refofMainList; 

        public ExpiringItem(T item, int interval, TimeUnit timeUnit, List<ExpiringItem<T>> list)
        {
            _item = item;
            _refofMainList = list;
            _timer = new Timer(interval.CalculateInterval(timeUnit).TotalMilliseconds);
            _timer.Start();
        }

        private void ElapsedEvent(object sender, ElapsedEventArgs e)
        {
            _timer.Elapsed -= ElapsedEvent;
            _refofMainList.Remove(this);
        }

        public T Item
        {
            get { return _item; }
        }
    }
}

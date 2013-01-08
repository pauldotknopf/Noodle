namespace Noodle.Events
{
    /// <summary>
    /// A common way of passing events to consumers.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventItem<T>
    {
        public EventItem()
        {
            
        }

        public EventItem(T item)
        {
            Item = item;
        }

        public T Item { get; protected set; }
    }
}

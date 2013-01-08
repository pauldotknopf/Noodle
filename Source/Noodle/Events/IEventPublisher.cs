namespace Noodle.Events
{
    /// <summary>
    /// Publish events to consumers to handle.
    /// </summary>
    public interface IEventPublisher
    {
        /// <summary>
        /// Publish an event to the consumers.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventMessage"></param>
        void Publish<T>(T eventMessage);
    }

    public static class EventPublisherExtensions
    {
        public static void EntityInserted<T>(this IEventPublisher eventPublisher, T entity)
        {
            eventPublisher.Publish(new EntityInserted<T>(entity));
        }
        public static void EntityUpdated<T>(this IEventPublisher eventPublisher, T entity)
        {
            eventPublisher.Publish(new EntityUpdated<T>(entity));
        }
        public static void EntityDeleted<T>(this IEventPublisher eventPublisher, T entity)
        {
            eventPublisher.Publish(new EntityDeleted<T>(entity));
        }
    }
}

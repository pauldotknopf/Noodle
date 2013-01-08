namespace Noodle.Events
{
    public class EmptyEventPublisher : IEventPublisher
    {
        public void Publish<T>(T eventMessage)
        {
            // do nothing
        }
    }
}

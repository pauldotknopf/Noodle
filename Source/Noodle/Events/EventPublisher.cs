using System;
using System.Linq;

namespace Noodle.Events
{
    public class EventPublisher : IEventPublisher
    {
        private readonly ISubscriptionService _subscriptionService;

        public EventPublisher(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        public void Publish<T>(T eventMessage)
        {
            var subscriptions = _subscriptionService.GetSubscriptions<T>();
            subscriptions.ToList().ForEach(x => PublishToConsumer(x, eventMessage));
        }

        private void PublishToConsumer<T>(IConsumer<T> x, T eventMessage)
        {
            try
            {
                x.Handle(eventMessage);
            }
            catch(Exception ex)
            {
                //TODO: NOtifier!
                //EngineContext.Resolve<ILogger>().Fatal(string.Format("There was an error in a consumer \"{0}\" for message \"{1}\".", x.GetType().FullName, eventMessage.GetType().FullName), ex);
            }
            finally
            {
                var instance = x as IDisposable;
                if (instance != null)
                {
                    instance.Dispose();
                }
            }
        }
    }
}

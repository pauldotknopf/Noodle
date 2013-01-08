using System.Collections.Generic;
using System.Linq;

namespace Noodle.Events
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IKernel _kernel;

        public SubscriptionService(IKernel kernel)
        {
            _kernel = kernel;
        }

        public IList<IConsumer<T>> GetSubscriptions<T>()
        {
            // Get all the sortered events
            return _kernel
                .GetAll<IConsumer<T>>()
                .OrderBy(x =>
                             {
                                 if (x is IConsumerSequence)
                                 {
                                     return (x as IConsumerSequence).Sequence - 1;
                                 }
                                 return int.MaxValue;
                             })
                .ToList();
        }
    }
}

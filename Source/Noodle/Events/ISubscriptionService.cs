using System.Collections.Generic;

namespace Noodle.Events
{
    /// <summary>
    /// Responsible for getting the consumers of a certain type.
    /// </summary>
    public interface ISubscriptionService
    {
        /// <summary>
        /// Get the consumers that handle "T"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IList<IConsumer<T>> GetSubscriptions<T>();
    }
}

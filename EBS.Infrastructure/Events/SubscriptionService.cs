using System.Collections.Generic;
using System.Collections;
using System;
namespace EBS.Infrastructure.Events
{
    /// <summary>
    /// Event subscription service
    /// </summary>
    public class SubscriptionService : ISubscriptionService
    {
        /// <summary>
        /// Get subscriptions
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Event consumers</returns>
        public IList<IConsumer<T>> GetSubscriptions<T>()
        {
           // return AppContext.Current.ResolveAll<IConsumer<T>>();
            throw new NotImplementedException("未实现"); 
        }
    }
}

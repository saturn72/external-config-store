using EasyCaching.Core;
using ExternalStore.Data;
using Microsoft.Extensions.Options;

namespace ExternalStore.Services.Subscription
{
    public class SubscriptionManager : ISubscriptionManager
    {
        private readonly IEasyCachingProvider _cache;
        private readonly ISubscriptionStore _store;

        public SubscriptionManager(
            IEasyCachingProvider cache,
            ISubscriptionStore store)
        {
            _cache = cache;
            _store = store;
        }
        public async Task AddSubscriptions(SubscriptionRequestContext context)
        {
            foreach (var req in context.Requests)
            {
                req.Handle = Guid.NewGuid().ToString("N");
                req.Expiration = req.RequestedExpiration != 0 ?
                    Math.Min(req.RequestedExpiration, context.Client.Expiration) :
                    req.RequestedExpiration;
            }
            //cache
            var groups = context.Requests.GroupBy(x => x.Expiration);

            foreach (var g in groups)
            {
                var d = g.ToDictionary(k => SubscriptionCaching.BuildGetByHandle(k.Handle));
                await _cache.SetAllAsync(d, TimeSpan.FromSeconds(g.Key));
            }
            await _store.Add(context.Requests);
        }
    }
}

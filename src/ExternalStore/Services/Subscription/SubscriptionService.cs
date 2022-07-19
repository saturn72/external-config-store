using EasyCaching.Core;
using ExternalStore.API.Events;
using ExternalStore.Data;
using ExternalStore.Domain;
using ExternalStore.Events;
using Microsoft.Extensions.Logging;

namespace ExternalStore.Services.Subscription
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IClientStore _clients;
        private readonly ISubscriptionStore _subscriptions;
        private readonly IEasyCachingProvider _cache;
        private readonly IEventPublisher _events;
        private readonly ILogger<SubscriptionService> _logger;

        public SubscriptionService(
            IClientStore clients,
            ISubscriptionStore subscriptions,
            IEasyCachingProvider cache,
            IEventPublisher events,
            ILogger<SubscriptionService> logger)
        {
            _clients = clients;
            _subscriptions = subscriptions;
            _events = events;
            _cache = cache;
            _logger = logger;
        }
        public async Task Subscribe(SubscriptionRequestContext context)
        {
            context.Client = await GetClientById(context.ClientId);

            if (context.Client == null)
            {
                context.Error = $"Failed to get client with \'id\'= {context.ClientId}";
                _logger.LogError(context.Error);
                return;
            }

            ValidateClientSubscriptionRequest(context);
            if (context.IsError)
            {
                _logger.LogError(context.Error);
                return;
            }

            foreach (var req in context.Requests)
            {
                req.Handle = Guid.NewGuid().ToString("N");
                req.Expiration = req.RequestedExpiration != 0 ?
                    Math.Min(req.RequestedExpiration, context.Client.Expiration) :
                    req.RequestedExpiration;
            }
            await _subscriptions.Add(context.Requests);
            _ = _events.Publish(EventKeys.Subscription.Added, context);
            
            //cache
            var groups = context.Requests.GroupBy(x => x.Expiration);

            foreach (var g in groups)
            {
                var d = g.ToDictionary(k => SubscriptionCaching.BuildGetByHandle(k.Handle));
                await _cache.SetAllAsync(d, TimeSpan.FromSeconds(g.Key));
            }
        }

        private async Task<Client?> GetClientById(string? clientId)
        {
            var key = ClientCaching.BuildGetById(clientId);
            var cv = await _cache.GetAsync(key,
                () => _clients.GetClientById(clientId),
                ClientCaching.Expiration);

            return cv.Value;
        }

        private void ValidateClientSubscriptionRequest(SubscriptionRequestContext context)
        {
            var configKeys = context.Requests.Select(s => s.ConfigKey).Distinct();
            var transports = context.Requests.Select(s => s.Transport).Distinct();

            var invalidConfigKeys = configKeys.Where(ck => !context.Client.ConfigKeys.Contains(ck));
            var invalidTransport = transports.Where(t => !context.Client.TransportNames.Contains(t));

            if (invalidConfigKeys.Any() || invalidTransport.Any())
                context.Error = $"Client is not allowed for config-key or/and transport. {nameof(invalidTransport)}: [{invalidTransport}], {nameof(invalidConfigKeys)}: [{invalidConfigKeys}]";
        }
    }
}

using EasyCaching.Core;
using ExternalStore.Data;
using ExternalStore.Domain;
using Microsoft.Extensions.Logging;

namespace ExternalStore.Services.Subscription
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IClientStore _store;
        private readonly IEasyCachingProvider _cache;
        private readonly ISubscriptionManager _subscriptionManager;
        private readonly ILogger<SubscriptionService> _logger;

        public SubscriptionService(
            IClientStore store,
            IEasyCachingProvider cache,
            ISubscriptionManager subscriptionManager,
            ILogger<SubscriptionService> logger)
        {
            _store = store;
            _cache = cache;
            _subscriptionManager = subscriptionManager;
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

            await _subscriptionManager.AddSubscriptions(context);
        }

        private async Task<Client?> GetClientById(string? clientId)
        {
            var key = ClientCaching.BuildGetById(clientId);
            var cv = await _cache.GetAsync(key,
                () => _store.GetClientById(clientId),
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

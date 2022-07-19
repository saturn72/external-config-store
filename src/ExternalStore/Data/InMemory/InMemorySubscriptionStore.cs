using ExternalStore.Services.Subscription;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ExternalStore.Data.InMemory
{
    public sealed class InMemorySubscriptionStore : ISubscriptionStore, IDisposable
    {
        private readonly List<SubscriptionDbModel> _subscriptions = new();
        private readonly ClearSubscriptionsOptions _options;
        private readonly ISystemClock _clock;
        private readonly Timer _timer;
        private readonly ILogger<InMemorySubscriptionStore> _logger;

        public InMemorySubscriptionStore(
            IOptions<ClearSubscriptionsOptions> options,
            ISystemClock clock,
            ILogger<InMemorySubscriptionStore> logger)
        {
            _options = options.Value;
            _clock = clock;
            _timer = new Timer(o => Handler(), null, _options.SecondsDueTime, _options.SecondsPeriod);
            _logger = logger;
        }

        public Task Add(IEnumerable<SubscriptionToPathRequest> requests)
        {
            var dbModels = requests.Select(toDbModel);
            _subscriptions.AddRange(dbModels);

            return Task.CompletedTask;

            SubscriptionDbModel toDbModel(SubscriptionToPathRequest request)
            {
                var epoch = _clock.UtcNow.ToUnixTimeSeconds();

                return new SubscriptionDbModel
                {
                    Id = Guid.NewGuid().ToString("N"),
                    ConfigKey = request.ConfigKey,
                    Path = request.Path,
                    Priority = request.Priority,
                    Transport = request.Transport,
                    Handle = request.Handle,
                    SubscribedAt = epoch,
                    AbsoluteExpiration = request.Expiration,
                    ExpiresAt = epoch + request.Expiration,
                    RequestedExpiration = request.RequestedExpiration,
                };
            }
        }

        public void Handler()
        {
            _logger.LogInformation($"{nameof(Handler)} is working.");

            var epoch = _clock.UtcNow.ToUnixTimeSeconds();
            _subscriptions.RemoveAll(s => epoch <= s.ExpiresAt);
        }

        public void Dispose()
        {
            _timer?.Change(Timeout.Infinite, 0);
            _timer?.Dispose();
        }

        private class SubscriptionDbModel
        {
            public string? Id { get; init; }
            public string? ConfigKey { get; init; }
            public string? Path { get; init; }
            public string? Priority { get; init; }
            public string? Transport { get; init; }
            public string? Handle { get; set; }
            public long SubscribedAt { get; set; }
            public uint AbsoluteExpiration { get; set; }
            public uint RequestedExpiration { get; set; }
            public long ExpiresAt { get; internal set; }
        }
    }
}

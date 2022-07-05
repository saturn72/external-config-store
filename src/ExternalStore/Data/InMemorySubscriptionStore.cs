using ExternalStore.Services.Subscription;
using Microsoft.AspNetCore.Authentication;

namespace ExternalStore.Data
{
    public sealed class InMemorySubscriptionStore : ISubscriptionStore
    {
        private readonly List<SubscriptionDbModel> _subscriptions = new();
        private readonly ISystemClock _clock;

        public InMemorySubscriptionStore(ISystemClock clock)
        {
            _clock = clock;
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
                    Expiration = request.Expiration,
                    ExpiresAt = epoch + request.Expiration,
                    RequestedExpiration = request.RequestedExpiration,
                };
            }
        }

        public Task ClearExpired()
        {
            var epoch = _clock.UtcNow.ToUnixTimeSeconds();
            _subscriptions.RemoveAll(s => epoch <= s.ExpiresAt);
            return Task.CompletedTask;
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
            public uint Expiration { get; set; }
            public uint RequestedExpiration { get; set; }
            public long ExpiresAt { get; internal set; }
        }
    }

}

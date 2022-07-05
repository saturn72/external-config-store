using ExternalStore.Domain;

namespace ExternalStore.Services.Subscription
{
    public record SubscriptionRequestContext : ContextBase
    {
        public IEnumerable<SubscriptionToPathRequest>? Requests { get; init; }
        public string? ClientId { get; init; }
        public Client? Client { get; set; }
    }
    public record SubscriptionToPathRequest
    {
        public string? ConfigKey { get; init; }
        public string? Path { get; init; }
        public string? Priority { get; init; }
        public string? Transport { get; init; }
        public string? Handle { get; set; }
        public DateTimeOffset SubscribedAt { get; set; }
        public uint Expiration { get; set; }
        public uint RequestedExpiration { get; set; }
    }
}

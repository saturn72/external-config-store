namespace ExternalStore.API.Models
{
    public record SubscriptionRequestModel
    {
        public string? ConfigKey { get; init; }
        public string? Path { get; init; }
        public string? Priority { get; init; }
        public string? Transport { get; init; }
    }
}

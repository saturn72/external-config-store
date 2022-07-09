namespace ExternalStore.Data
{
    public sealed class ClearSubscriptionsOptions
    {
        public const string Section = "clear-subscriptions";
        public long SecondsPeriod { get; init; } = 0;
        public long SecondsDueTime { get; init; } = 10;
    }
}

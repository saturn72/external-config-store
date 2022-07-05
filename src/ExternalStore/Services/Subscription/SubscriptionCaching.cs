namespace ExternalStore.Services.Subscription
{
    internal class SubscriptionCaching
    {
        internal const string Prefix = "subscriptions:";
        internal static string BuildGetByHandle(string? handle) => $"{Prefix}handle={handle}";
    }
}

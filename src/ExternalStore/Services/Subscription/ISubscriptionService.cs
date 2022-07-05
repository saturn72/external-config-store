namespace ExternalStore.Services.Subscription
{
    public interface ISubscriptionService
    {
        Task Subscribe(SubscriptionRequestContext context);
    }
}

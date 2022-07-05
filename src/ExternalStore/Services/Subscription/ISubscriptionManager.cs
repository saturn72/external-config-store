namespace ExternalStore.Services.Subscription
{
    public interface ISubscriptionManager
    {
        Task AddSubscriptions(SubscriptionRequestContext context);
    }

}

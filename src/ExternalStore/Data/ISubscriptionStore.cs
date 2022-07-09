using ExternalStore.Services.Subscription;

namespace ExternalStore.Data
{
    public interface ISubscriptionStore
    {
        Task Add(IEnumerable<SubscriptionToPathRequest> requests);
    }
}

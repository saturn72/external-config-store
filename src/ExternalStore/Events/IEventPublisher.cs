using ExternalStore.Domain;

namespace ExternalStore.API.Events
{
    public interface IEventPublisher
    {
        Task Publish<TContext>(string key, TContext context) where TContext : ContextBase;
        void Subscribe(string key, Func<ContextBase, IServiceProvider, Task> handler);
    }
}

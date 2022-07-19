using ExternalStore.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ExternalStore.API.Events
{
    public class DefaultEventPublisher : IEventPublisher
    {
        private readonly Dictionary<string, List<Func<ContextBase, IServiceProvider, Task>>> _subscriptions = new();
        private readonly IServiceProvider _services;
        private readonly ILogger<DefaultEventPublisher> _logger;
        private const int TaskExecutionTimeout = 60000;

        public DefaultEventPublisher(
            IServiceProvider services,
            ILogger<DefaultEventPublisher> logger)
        {
            _services = services;
            _logger = logger;
        }

        public async Task Publish<TContext>(string key, TContext context) where TContext : ContextBase
        {
            if (!_subscriptions.TryGetValue(key, out var handlers))
                return;

            await Task.Yield();

            var tasks = new List<Task>();
            _logger.LogDebug($"Publishing event to handlers. Key = {key}");

            using var scope = _services.CreateScope();
            var sp = scope.ServiceProvider;
            foreach (var h in handlers)
                tasks.Add(h(context, sp));

            if (!Task.WaitAll(tasks.ToArray(), TaskExecutionTimeout))
                _logger.LogWarning($"Not all tasks finished execution within given timout ({TaskExecutionTimeout})");
        }
        public void Subscribe(string key, Func<ContextBase, IServiceProvider, Task> handler)
        {
            if (!_subscriptions.TryGetValue(key, out var handlers))
                _subscriptions[key] = new();

            _subscriptions[key].Add(handler);
        }
    }
}

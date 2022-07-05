using ExternalStore.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ExternalStore.HostedServices
{
    public class ClearSubscriptionsOptions
    {
        public long SecondsPeriod { get; init; } = 0;
        public long SecondsDueTime { get; init; } = 10;

        public Action<IServiceProvider> Handler { get; set; } = services =>
        {
            var logger = services.GetService<ILogger<ClearSubscriptionsOptions>>();
            logger.LogInformation($"{nameof(Handler)} is working.");

            var store = services.GetService<ISubscriptionStore>();
            _ = store.ClearExpired();
        };
    }
}

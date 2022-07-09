using ExternalStore.Data;
using ExternalStore.Domain;
using ExternalStore.Services.Config;
using ExternalStore.Services.Subscription;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddExternalConfigStore(this IServiceCollection services)
        {
            services.TryAddSingleton<IConfigService, ConfigService>();
            services.TryAddSingleton<ISubscriptionService, SubscriptionService>();
            return services;
        }

        public static IServiceCollection AddInMemoryStore(this IServiceCollection services, IEnumerable<Client> clients)
        {
            services.AddSingleton<IClientStore>(sp => new InMemoryClientStore(clients));

            return services;
        }

        public static IServiceCollection AddInMemorySubscriptionStore(this IServiceCollection services, IConfiguration configuration, string section = ClearSubscriptionsOptions.Section)
        {
            services.AddAuthentication();
            services.AddSingleton<ISubscriptionStore, InMemorySubscriptionStore>();
            services.AddOptions<ClearSubscriptionsOptions>()
                .Bind(configuration.GetSection(section));

            return services;
        }
    }
}

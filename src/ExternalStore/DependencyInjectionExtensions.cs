using ExternalStore.API.Configurars;
using ExternalStore.API.Events;
using ExternalStore.Data;
using ExternalStore.Data.Files;
using ExternalStore.Data.InMemory;
using ExternalStore.Domain;
using ExternalStore.Services.Config;
using ExternalStore.Services.Subscription;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddExternalConfigStore(this IServiceCollection services)
        {
            services.TryAddSingleton<IConfigService, ConfigService>();
            services.TryAddSingleton<ISubscriptionService, SubscriptionService>();
            services.TryAddSingleton<IEventPublisher, DefaultEventPublisher>();
            services.AddSingleton<IConfigStoreFactory, DefaultConfigStoreFactory>();

            return services;
        }

        public static IServiceCollection AddInMemoryStore(
            this IServiceCollection services,
            IEnumerable<Client> clients)
        {
            services.AddSingleton<IClientStore>(sp => new InMemoryClientStore(clients));

            return services;
        }

        public static IServiceCollection AddInMemorySubscriptionStore(
            this IServiceCollection services,
            IConfiguration configuration,
            string section = ClearSubscriptionsOptions.Section)
        {
            services.AddAuthentication();
            services.AddSingleton<ISubscriptionStore, InMemorySubscriptionStore>();
            services.AddOptions<ClearSubscriptionsOptions>()
                .Bind(configuration.GetSection(section));

            return services;
        }

        public static IServiceCollection AddFileConfigStore(
            this IServiceCollection services,
            Action<FileConfigStoreOptions>? config = null)
        {
            var options = new FileConfigStoreOptions();
            config?.Invoke(options);

            return services.AddSingleton<IConfigStore>(sp =>
                new FileConfigStore(
                    options,
                    sp.GetService<ILogger<FileConfigStore>>())
                );
        }
    }
}

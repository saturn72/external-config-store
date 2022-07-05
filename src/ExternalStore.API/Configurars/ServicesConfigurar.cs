using ExternalStore.Services.Config;
using ExternalStore.Services.Subscription;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ExternalStore.API.Configurars
{
    public class ServicesConfigurar
    {
        public void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.TryAddSingleton<IConfigService, ConfigService>();
            services.TryAddSingleton<ISubscriptionService, SubscriptionService>();
        }
    }
}

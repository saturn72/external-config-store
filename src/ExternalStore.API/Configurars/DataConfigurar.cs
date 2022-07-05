using ExternalStore.Data;
using ExternalStore.Data.MySql;
using ExternalStore.HostedServices;

namespace ExternalStore.API.Configurars
{
    public class DataConfigurar
    {
        public void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConfigStore>(sp =>
            {
                var cs = configuration["CONNECTION_STRING"] ?? throw new ArgumentNullException("CONNECTION_STRING");
                return new MySqlConfigStore(cs);
            });

            services.AddSingleton<IClientStore>(sp => new InMemoryClientStore(Config.Clients));
            services.AddSingleton<ISubscriptionStore, InMemorySubscriptionStore>();
            services.AddHostedService<ClearSubscriptionsTimedHostedService>();
        }
    }
}

namespace ExternalStore.API.Configurars
{
    public class EasyCachingConfigurar
    {
        private const string DefaultCachingProviderName = "inmemory-default";
        public void Configure(IServiceCollection services)
        {
            services.AddEasyCaching(options =>
            {
                options.UseInMemory(DefaultCachingProviderName);
            });
        }
    }
}

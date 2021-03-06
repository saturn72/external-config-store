using ExternalStore.API.Configurars;

namespace ExternalStore.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services
                .AddExternalConfigStore()
                .AddInMemoryStore(Config.Clients)
                .AddFileConfigStore(o => o.EnableNotifications = false)
                .AddInMemorySubscriptionStore(builder.Configuration);

            new EasyCachingConfigurar().Configure(builder.Services);

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
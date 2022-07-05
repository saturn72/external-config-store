using ExternalStore.API.Configurars;

namespace ExternalStore.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            new DataConfigurar().Configure(builder.Services, builder.Configuration);
            new EasyCachingConfigurar().Configure(builder.Services);
            new ServicesConfigurar().Configure(builder.Services, builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
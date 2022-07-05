using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ExternalStore.HostedServices
{
    public sealed class ClearSubscriptionsTimedHostedService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<ClearSubscriptionsTimedHostedService> _logger;
        private Timer? _timer = null;

        public ClearSubscriptionsTimedHostedService(
            IServiceProvider services,
            ILogger<ClearSubscriptionsTimedHostedService> logger)
        {
            _services = services;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(ClearSubscriptionsTimedHostedService)} is running.");
            var ov = _services.GetService<IOptions<ClearSubscriptionsOptions>>();
            var options = ov.Value;

            _timer = new Timer(o => options.Handler(_services), null, options.SecondsDueTime, options.SecondsPeriod);

            return Task.CompletedTask;
        }

        private void ClearWorker(object? state)
        {
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(ClearSubscriptionsTimedHostedService)} is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
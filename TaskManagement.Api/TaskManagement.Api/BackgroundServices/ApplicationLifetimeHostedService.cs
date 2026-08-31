using Microsoft.Extensions.Hosting;

namespace TaskManagement.Api.BackgroundServices
{
    public class ApplicationLifetimeHostedService : IHostedService //Hosted Service olduğunu belirtir.
    {
        private readonly ILogger<ApplicationLifetimeHostedService> _logger;

        public ApplicationLifetimeHostedService(ILogger<ApplicationLifetimeHostedService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken) //uygulama başlarken çalışır.
        {
            _logger.LogInformation("ApplicationLifetimeHostedService başladı.");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)//uygulama kapanırken çalışır.
        {
            _logger.LogInformation("ApplicationLifetimeHostedService duruyor.");
            return Task.CompletedTask;
        }
    }
}

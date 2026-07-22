using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using TaskManagement.Api.Settings;

namespace TaskManagement.Api.HealthChecks
{
    public class ConfigurationHealthCheck : IHealthCheck
    {
        private readonly ApplicationSettings _settings;

        public ConfigurationHealthCheck(
            IOptions<ApplicationSettings> options)
        {
            _settings = options.Value;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(_settings.ApplicationName))
            {
                return Task.FromResult(
                    HealthCheckResult.Unhealthy("ApplicationName is empty."));
            }

            return Task.FromResult(
                HealthCheckResult.Healthy("Configuration loaded."));
        }
    }
}

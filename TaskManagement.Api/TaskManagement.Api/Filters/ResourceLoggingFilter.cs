using Microsoft.AspNetCore.Mvc.Filters;

namespace TaskManagement.Api.Filters
{
    public class ResourceLoggingFilter: IResourceFilter
    {
        private readonly ILogger<ResultLoggingFilter> _logger;

        public ResourceLoggingFilter(ILogger<ResultLoggingFilter> logger)
        {
            _logger = logger;
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            _logger.LogInformation("Resource başladı.");
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            _logger.LogInformation("Resource tamamlandı.");
        }
    }
}

using Microsoft.AspNetCore.Mvc.Filters;

namespace TaskManagement.Api.Filters
{
    public class SecondActionFilter : IAsyncActionFilter
    {
        private readonly ILogger<SecondActionFilter> _logger;

        public SecondActionFilter(
            ILogger<SecondActionFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            _logger.LogInformation(
                "SECOND FILTER - BEFORE");

            await next();

            _logger.LogInformation(
                "SECOND FILTER - AFTER");
        }
    }
}
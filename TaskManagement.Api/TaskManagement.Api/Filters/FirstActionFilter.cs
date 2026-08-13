using Microsoft.AspNetCore.Mvc.Filters;

namespace TaskManagement.Api.Filters
{
    public class FirstActionFilter : IAsyncActionFilter
    {
        private readonly ILogger<FirstActionFilter> _logger;

        public FirstActionFilter(
            ILogger<FirstActionFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            _logger.LogInformation(
                "FIRST FILTER - BEFORE");

            await next();

            _logger.LogInformation(
                "FIRST FILTER - AFTER");
        }
    }
}
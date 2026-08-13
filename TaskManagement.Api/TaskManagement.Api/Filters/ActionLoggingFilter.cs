using Microsoft.AspNetCore.Mvc.Filters;

namespace TaskManagement.Api.Filters
{
    public class ActionLoggingFilter : IAsyncActionFilter //sınıfımızı Action Filter yapıyor.
    {
        private readonly ILogger<ActionLoggingFilter> _logger;

        public ActionLoggingFilter(ILogger<ActionLoggingFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var controllerName = context.ActionDescriptor.RouteValues["controller"];
            var actionName = context.ActionDescriptor.RouteValues["action"];

            _logger.LogInformation("Action başladı: {Controller}/{Action}",controllerName,actionName);

            await next();

            _logger.LogInformation("Action tamamlandı: {Controller}/{Action}", controllerName, actionName);
        }
    }
}

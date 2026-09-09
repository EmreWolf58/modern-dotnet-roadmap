namespace TaskManagement.Api.EndpointFilters
{
    public class LoggingEndpointFilter : IEndpointFilter //Bir Endpoint Filter oluşturmak için bunu implement ediyoruz.
    {
        private readonly ILogger<LoggingEndpointFilter> _logger;

        public LoggingEndpointFilter(ILogger<LoggingEndpointFilter> logger)
        {
            _logger = logger;
        }

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            _logger.LogInformation("Minimal API endpoint çalışmadan önce.");
            var result = await next(context); // next: Pipeline'daki sonraki adıma devam et.
            _logger.LogInformation("Minimal API endpoint çalıştıktan sonra.");
            return result;
        }
    }
}

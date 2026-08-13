using Microsoft.AspNetCore.Mvc.Filters;

namespace TaskManagement.Api.Filters
{
    public class ResultLoggingFilter: IResultFilter
    {
        private readonly ILogger<ResultLoggingFilter> _logger;

        public ResultLoggingFilter(ILogger<ResultLoggingFilter> logger)
        {
            _logger = logger;
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            _logger.LogInformation("Result hazırlanıyor: {ResultType}", context.Result.GetType().Name);
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            _logger.LogInformation("Result Tamamlandı.");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TaskManagement.Api.Filters
{
    public class TestExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<TestExceptionFilter> _logger;

        public TestExceptionFilter(ILogger<TestExceptionFilter> logger)
        {
            _logger = logger;
        }
        
        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "Exception filter çalıştı.");

            context.Result = new ObjectResult(new
            {
                Message = "Exception Filter hatayı yakaladı."
            })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };

            context.ExceptionHandled = true; // Exception'ın işlenmiş olduğunu belirtir.
        }
    }
}

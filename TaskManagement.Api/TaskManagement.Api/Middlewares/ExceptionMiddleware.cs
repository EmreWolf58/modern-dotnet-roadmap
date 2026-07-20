using TaskManagement.Api.Responses;

namespace TaskManagement.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next; // _next, bir sonraki middleware'i temsil eden bir RequestDelegate türünde bir değişkendir. 
        private readonly ILogger<ExceptionMiddleware> _logger; // _logger, loglama işlemleri için kullanılan bir ILogger türünde bir değişkendir.

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync (HttpContext context)// InvokeAsync metodu, middleware'in çalıştırılmasını sağlayan asenkron bir metottur.
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Beklenmeyen hata oluştu. Path: {Path}", context.Request.Path);
                if (context.Response.HasStarted)
                {
                    throw;
                }
                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                var response = new ApiErrorResponse
                {
                    Message= "Beklenmeyen bir hata oluştu.",
                    StatusCode=StatusCodes.Status500InternalServerError,
                    Path = context.Request.Path,
                    TraceId = context.TraceIdentifier,
                };
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}

using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TaskManagement.Api.Responses;

namespace TaskManagement.Api.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var validationErrors = new List<ValidationFailure>();

            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument is null)
                {
                    continue;
                }

                var argumentType = argument.GetType();

                var validatorType =
                    typeof(IValidator<>).MakeGenericType(argumentType);

                var validator =
                    _serviceProvider.GetService(validatorType);

                // Bu model için validator yoksa devam et.
                if (validator is null)
                {
                    continue;
                }

                var validateMethod = validatorType.GetMethod(
                    "ValidateAsync",
                    new[]
                    {
                        argumentType,
                        typeof(CancellationToken)
                    });

                if (validateMethod is null)
                {
                    continue;
                }

                var validationTask =
                    (Task<ValidationResult>?)validateMethod.Invoke(
                        validator,
                        new object[]
                        {
                            argument,
                            context.HttpContext.RequestAborted
                        });

                if (validationTask is null)
                {
                    continue;
                }

                var validationResult =
                    await validationTask;

                validationErrors.AddRange(
                    validationResult.Errors);
            }

            if (validationErrors.Count > 0)
            {
                var errors = validationErrors
                    .GroupBy(error => error.PropertyName)
                    .ToDictionary(
                        group => group.Key,
                        group => group
                            .Select(error => error.ErrorMessage)
                            .Distinct()
                            .ToArray());

                var response = new ApiErrorResponse
                {
                    Message = "Validation hatası oluştu.",
                    StatusCode =
                        StatusCodes.Status400BadRequest,
                    Errors = errors,
                    Path =
                        context.HttpContext.Request.Path,
                    TraceId =
                        context.HttpContext.TraceIdentifier
                };

                context.Result =
                    new BadRequestObjectResult(response);

                return;
            }

            await next();
        }
    }
}
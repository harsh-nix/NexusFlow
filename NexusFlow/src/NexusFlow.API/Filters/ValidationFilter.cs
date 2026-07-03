using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NexusFlow.Application.DTOs.Common;

namespace NexusFlow.API.Filters
{
    /// <summary>
    /// Runs before every controller action. For each action argument that has
    /// a matching FluentValidation IValidator&lt;T&gt; registered in DI, the
    /// argument is validated. If validation fails, the pipeline short-circuits
    /// with a 400 response shaped like every other ApiResponse in this API —
    /// there is no need to call validators manually inside services/controllers.
    /// (FluentValidation.AspNetCore is unmaintained, so this replaces it.)
    /// </summary>
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
            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument == null)
                    continue;

                var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());

                if (_serviceProvider.GetService(validatorType) is not IValidator validator)
                    continue;

                var validationContext = new ValidationContext<object>(argument);
                var result = await validator.ValidateAsync(validationContext);

                if (!result.IsValid)
                {
                    var errors = result.Errors
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    context.Result = new BadRequestObjectResult(
                        ApiResponse<object>.Fail(errors, 400));
                    return;
                }
            }

            await next();
        }
    }
}
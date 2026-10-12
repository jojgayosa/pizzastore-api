using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PizzaStore.API.Common;

public class ValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null)
            {
                continue;
            }

            var argumentType = argument.GetType();

            var validatorType = typeof(IValidator<>)
                .MakeGenericType(argumentType);

            var validator = context.HttpContext.RequestServices
                .GetService(validatorType);

            if (validator is null)
            {
                continue;
            }

            var validationResult = await ValidateAsync(
                validator,
                argument,
                context.HttpContext.RequestAborted);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
        }

        await next();
    }

    private static async Task<FluentValidation.Results.ValidationResult> ValidateAsync(
        object validator,
        object argument,
        CancellationToken cancellationToken)
    {
        return await ((IValidator)validator)
            .ValidateAsync(
                new ValidationContext<object>(argument),
                cancellationToken);
    }
}
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PizzaStore.Application.Common.Exceptions;

namespace PizzaStore.API.Common;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        ProblemDetails problem;

        switch (exception)
        {
            case BadRequestException ex:
                problem = new ProblemDetails
                {
                    Title = "Bad Request",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = ex.Message
                };
                break;

            case ValidationException ex:
                problem = new ProblemDetails
                {
                    Title = "One or more validation errors occurred.",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "See the errors property for details."
                };

                problem.Extensions["errors"] = ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray());

                break;

            case NotFoundException ex:
                problem = new ProblemDetails
                {
                    Title = "Not Found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = ex.Message
                };
                break;

            case ConflictException ex:
                problem = new ProblemDetails
                {
                    Title = "Conflict",
                    Status = StatusCodes.Status409Conflict,
                    Detail = ex.Message
                };
                break;

            default:
                _logger.LogError(
                    exception,
                    "An unexpected exception occurred.");

                problem = new ProblemDetails
                {
                    Title = "Internal Server Error",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = "An unexpected error occurred."
                };
                break;
        }

        context.Response.StatusCode = problem.Status!.Value;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(
            problem,
            cancellationToken);

        return true;
    }
}
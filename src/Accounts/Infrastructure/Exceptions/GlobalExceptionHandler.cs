using Accounts.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Accounts.Infrastructure.Exceptions
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger = logger;

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

            if (exception is ValidationException validationException)
            {
                var errors = validationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray());

                var validationProblemDetails = new ValidationProblemDetails(errors)
                {
                    Status = StatusCodes.Status400BadRequest
                };

                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await httpContext.Response.WriteAsJsonAsync(validationProblemDetails, cancellationToken);
                return true;
            }

            var (statusCode, title) = exception switch
            {
                CurrenciesMissmatchException => (400, "Currency mismatch"),
                InsufficientFundsException => (422, "Insufficient funds"),
                AccountNotActiveException => (422, "Account not active"),
                AccountNotEmptyException => (422, "Account not empty"),
                CurrencyMismatchException => (400, "Currency mismatch"),
                AccountNotFoundException => (404, "Account not found"),
                AccountOwnershipException => (403, "Forbidden"),

                DomainValidationException => (400, "Validation error"),
                ValueEmptyException => (500, "Internal server error"),

                _ => (500, "Internal server error")
            };

            var details = exception.Message;
            if (statusCode == 500) details = "An error has occured. If the issue persists, please contact customer support.";

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = details
            };

            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}

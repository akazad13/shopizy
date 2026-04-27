using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopizy.Api.Common.LoggerMessages;
using Shopizy.SharedKernel.Application.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Shopizy.Api.Common.Errors;

[ExcludeFromCodeCoverage]
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(exception);

        var traceId = httpContext.TraceIdentifier;
        var problem = ResolveProblem(exception);

        // Log at the right level: client-side problems are info-level, server-side are error.
        // Sanitize the message to avoid leaking PII (emails, phone numbers, tokens, card numbers).
        if (problem.IsServerSide)
        {
            _logger.UnhandledExceptionError(exception, LogSanitizer.Sanitize(exception.Message));
        }

        httpContext.Response.StatusCode = problem.StatusCode;
        httpContext.Response.ContentType = "application/problem+json";

        var details = new ProblemDetails
        {
            Status = problem.StatusCode,
            Title = problem.Title,
            Detail = problem.Detail,
            Type = problem.Type,
            Extensions = { ["traceId"] = traceId }
        };

        if (problem.Errors is not null)
        {
            details.Extensions["errors"] = problem.Errors;
        }

        await httpContext.Response.WriteAsJsonAsync(details, cancellationToken);
        return true;
    }

    private static ProblemDescriptor ResolveProblem(Exception exception) => exception switch
    {
        ValidationException ve => new ProblemDescriptor(
            StatusCode: StatusCodes.Status400BadRequest,
            Title: "Validation failed",
            Detail: "One or more validation rules failed.",
            Type: "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            IsServerSide: false,
            Errors: ve.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }).Cast<object>().ToArray()),

        DbUpdateConcurrencyException => new ProblemDescriptor(
            StatusCode: StatusCodes.Status409Conflict,
            Title: "Conflict",
            Detail: "The resource was modified by another request. Reload and retry.",
            Type: "https://tools.ietf.org/html/rfc7231#section-6.5.8",
            IsServerSide: false),

        DbUpdateException => new ProblemDescriptor(
            StatusCode: StatusCodes.Status500InternalServerError,
            Title: "Database error",
            Detail: "A database error occurred while processing the request.",
            Type: "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            IsServerSide: true),

        OperationCanceledException or TaskCanceledException => new ProblemDescriptor(
            StatusCode: 499, // Client Closed Request (nginx convention)
            Title: "Client Closed Request",
            Detail: "The request was cancelled before completion.",
            Type: null,
            IsServerSide: false),

        UnauthorizedAccessException => new ProblemDescriptor(
            StatusCode: StatusCodes.Status401Unauthorized,
            Title: "Unauthorized",
            Detail: "Authentication is required to access this resource.",
            Type: "https://tools.ietf.org/html/rfc7235#section-3.1",
            IsServerSide: false),

        ArgumentException => new ProblemDescriptor(
            StatusCode: StatusCodes.Status400BadRequest,
            Title: "Bad Request",
            Detail: "The request was malformed.",
            Type: "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            IsServerSide: false),

        InvalidOperationException => new ProblemDescriptor(
            StatusCode: StatusCodes.Status400BadRequest,
            Title: "Invalid operation",
            Detail: "The request could not be processed in the current state.",
            Type: "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            IsServerSide: false),

        TimeoutException => new ProblemDescriptor(
            StatusCode: StatusCodes.Status503ServiceUnavailable,
            Title: "Service Unavailable",
            Detail: "An upstream operation timed out.",
            Type: "https://tools.ietf.org/html/rfc7231#section-6.6.4",
            IsServerSide: true),

        _ => new ProblemDescriptor(
            StatusCode: StatusCodes.Status500InternalServerError,
            Title: "An unexpected error occurred.",
            Detail: "An unexpected error occurred while processing the request.",
            Type: "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            IsServerSide: true),
    };

    private sealed record ProblemDescriptor(
        int StatusCode,
        string Title,
        string Detail,
        string? Type,
        bool IsServerSide,
        object[]? Errors = null);
}

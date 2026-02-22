using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Shopizy.Contracts.Common;
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
        _logger.LogError(exception, "An unhandled exception has occurred: {Message}", exception.Message);

        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(new { message = "Error occured!", errors = new string[] { exception.Message } }, cancellationToken);

        return true;
    }
}

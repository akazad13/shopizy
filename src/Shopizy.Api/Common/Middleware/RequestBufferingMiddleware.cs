namespace Shopizy.Api.Common.Middleware;

/// <summary>
/// Enables request-body buffering on mutation requests so downstream endpoint filters
/// (e.g. the idempotency filter) can re-read the body after parameter binding has consumed it.
/// </summary>
public sealed class RequestBufferingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (
            HttpMethods.IsPost(context.Request.Method)
            || HttpMethods.IsPut(context.Request.Method)
            || HttpMethods.IsPatch(context.Request.Method)
        )
        {
            context.Request.EnableBuffering();
        }

        await next(context);
    }
}

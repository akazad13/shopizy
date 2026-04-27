using System.Security.Cryptography;
using Shopizy.Application.Common.Interfaces.Services;

namespace Shopizy.Api.Common.Idempotency;

public sealed class IdempotencyEndpointFilter(IIdempotencyStore store) : IEndpointFilter
{
    private const string HeaderName = "Idempotency-Key";
    private static readonly TimeSpan DefaultTtl = TimeSpan.FromHours(24);

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);

        var http = context.HttpContext;

        if (!http.Request.Headers.TryGetValue(HeaderName, out var headerValue) || string.IsNullOrWhiteSpace(headerValue))
        {
            return Results.Problem(
                title: "Missing Idempotency-Key header",
                detail: $"This endpoint requires an '{HeaderName}' request header.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var idempotencyKey = headerValue.ToString();
        var requestHash = await ComputeRequestHashAsync(http);
        var scopedKey = ScopeKey(http, idempotencyKey);

        var existing = await store.TryGetAsync(scopedKey, http.RequestAborted);
        if (existing is not null)
        {
            if (!string.Equals(existing.RequestHash, requestHash, StringComparison.Ordinal))
            {
                return Results.Problem(
                    title: "Idempotency-Key conflict",
                    detail: "This Idempotency-Key was reused with a different request body.",
                    statusCode: StatusCodes.Status409Conflict);
            }
            return new ReplayResult(existing);
        }

        var originalBody = http.Response.Body;
        await using var buffer = new MemoryStream();
        http.Response.Body = buffer;

        try
        {
            var result = await next(context);
            if (result is IResult inner)
            {
                await inner.ExecuteAsync(http);
            }
            else if (result is not null)
            {
                await http.Response.WriteAsJsonAsync(result, http.RequestAborted);
            }

            await http.Response.Body.FlushAsync(http.RequestAborted);
            var bytes = buffer.ToArray();

            if (http.Response.StatusCode is >= 200 and < 300)
            {
                var record = new IdempotencyRecord(
                    RequestHash: requestHash,
                    StatusCode: http.Response.StatusCode,
                    ContentType: http.Response.ContentType ?? "application/json",
                    Body: bytes);
                await store.StoreAsync(scopedKey, record, DefaultTtl, http.RequestAborted);
            }

            await originalBody.WriteAsync(bytes, http.RequestAborted);
            return EmptyHttpResult.Instance;
        }
        finally
        {
            http.Response.Body = originalBody;
        }
    }

    private static string ScopeKey(HttpContext http, string idempotencyKey)
    {
        var userId = http.User?.Identity?.IsAuthenticated == true
            ? http.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "anon"
            : "anon";
        var path = http.Request.Path.HasValue ? http.Request.Path.Value!.ToLowerInvariant() : "/";
        return $"{userId}:{path}:{idempotencyKey}";
    }

    private static async Task<string> ComputeRequestHashAsync(HttpContext http)
    {
        http.Request.EnableBuffering();
        http.Request.Body.Position = 0;

        using var sha = SHA256.Create();
        var buffer = new byte[8192];
        int read;
        while ((read = await http.Request.Body.ReadAsync(buffer, http.RequestAborted)) > 0)
        {
            sha.TransformBlock(buffer, 0, read, null, 0);
        }
        sha.TransformFinalBlock([], 0, 0);

        http.Request.Body.Position = 0;
        return Convert.ToHexString(sha.Hash!);
    }

    private sealed class ReplayResult(IdempotencyRecord record) : IResult
    {
        public async Task ExecuteAsync(HttpContext httpContext)
        {
            ArgumentNullException.ThrowIfNull(httpContext);
            httpContext.Response.StatusCode = record.StatusCode;
            httpContext.Response.ContentType = record.ContentType;
            await httpContext.Response.Body.WriteAsync(record.Body, httpContext.RequestAborted);
        }
    }

    private sealed class EmptyHttpResult : IResult
    {
        public static readonly EmptyHttpResult Instance = new();
        public Task ExecuteAsync(HttpContext httpContext) => Task.CompletedTask;
    }
}

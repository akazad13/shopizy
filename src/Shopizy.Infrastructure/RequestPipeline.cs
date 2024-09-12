using Microsoft.AspNetCore.Builder;
using Shopizy.Infrastructure.Common.Middleware;

namespace Shopizy.Infrastructure;

public static class RequestPipeline
{
    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        _ = app.UseMiddleware<EventualConsistencyMiddleware>();
        return app;
    }
}

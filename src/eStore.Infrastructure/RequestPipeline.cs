using eStore.Infrastructure.Common.Middleware;
using Microsoft.AspNetCore.Builder;

namespace eStore.Infrastructure;

public static class RequestPipeline
{
    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        app.UseMiddleware<EventualConsistencyMiddleware>();
        return app;
    }
}

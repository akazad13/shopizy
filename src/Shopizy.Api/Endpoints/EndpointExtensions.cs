using System.Reflection;
using Asp.Versioning;

namespace Shopizy.Api.Endpoints;

public static class EndpointExtensions
{
    public static IServiceCollection AddEndpoints(
        this IServiceCollection services,
        Assembly assembly
    )
    {
        var endpointTypes = assembly
            .GetTypes()
            .Where(t => typeof(IEndpoint).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var endpointType in endpointTypes)
        {
            services.AddTransient(typeof(IEndpoint), endpointType);
        }

        return services;
    }

    public static IApplicationBuilder MapEndpoints(this WebApplication app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        var group = app.MapGroup(string.Empty).WithApiVersionSet(versionSet);

        var endpoints = app.Services.CreateScope().ServiceProvider.GetServices<IEndpoint>();

        foreach (var endpoint in endpoints)
        {
            endpoint.MapEndpoint(group);
        }

        return app;
    }
}

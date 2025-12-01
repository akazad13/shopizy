using Mapster;
using MapsterMapper;

namespace Shopizy.Api.Common.Mapping;

/// <summary>
/// Dependency injection for mapping configurations.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds mapping configurations to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection with mappings added.</returns>
    public static IServiceCollection AddMappings(this IServiceCollection services)
    {
        TypeAdapterConfig config = TypeAdapterConfig.GlobalSettings;
        config.Scan(typeof(DependencyInjection).Assembly);

        services.AddSingleton(config).AddScoped<IMapper, ServiceMapper>();
        return services;
    }
}

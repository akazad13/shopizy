using Mapster;
using MapsterMapper;

namespace Shopizy.Api.Common.Mapping;

public static class DependencyInjection
{
    public static IServiceCollection AddMappings(this IServiceCollection services)
    {
        TypeAdapterConfig config = TypeAdapterConfig.GlobalSettings;
        config.Scan(typeof(DependencyInjection).Assembly);

        services.AddSingleton(config).AddScoped<IMapper, ServiceMapper>();
        return services;
    }
}

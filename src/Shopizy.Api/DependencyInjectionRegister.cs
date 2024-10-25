using Shopizy.Api.Common.Mapping;

namespace Shopizy.Api;

public static class DependencyInjectionRegister
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer().AddSwaggerGen();
        services.AddMappings();

        return services;
    }
}

using Shopizy.Api.Common.Mapping;

namespace Shopizy.Api;

public static class DependencyInjectionRegister
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        _ = services.AddControllers();

        _ = services.AddEndpointsApiExplorer();
        _ = services.AddSwaggerGen();

        _ = services.AddMappings();

        return services;
    }
}

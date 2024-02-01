using Microsoft.Extensions.DependencyInjection;

namespace Shopizy.Application;

public static class DependencyInjectionRegister
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(
            msc => msc.RegisterServicesFromAssembly(typeof(DependencyInjectionRegister).Assembly)
        );
        return services;
    }
}

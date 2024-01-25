using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace eStore.Application;

public static class DependencyInjectionRegister
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(
            msc => msc.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
        );
        return services;
    }
}

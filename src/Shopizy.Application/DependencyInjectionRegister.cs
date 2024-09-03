using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Shopizy.Application.Common.Behaviors;

namespace Shopizy.Application;

public static class DependencyInjectionRegister
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        _ = services.AddMediatR(msc =>
        {
            _ = msc.RegisterServicesFromAssembly(typeof(DependencyInjectionRegister).Assembly);
            _ = msc.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
            _ = msc.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        _ = services.AddValidatorsFromAssemblyContaining(typeof(DependencyInjectionRegister));

        return services;
    }
}

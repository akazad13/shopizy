using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using shopizy.Application.Common.Behaviors;

namespace Shopizy.Application;

public static class DependencyInjectionRegister
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(msc =>
        {
            msc.RegisterServicesFromAssembly(typeof(DependencyInjectionRegister).Assembly);
            msc.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        services.AddValidatorsFromAssemblyContaining(typeof(DependencyInjectionRegister));

        return services;
    }
}

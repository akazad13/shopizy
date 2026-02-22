using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shopizy.SharedKernel.Application.Behaviors;
using Shopizy.Application.Common.Security.CurrentUser;

namespace Shopizy.Application;

public static class DependencyInjectionRegister
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddMediatR(msc =>
        {
            msc.RegisterServicesFromAssembly(typeof(DependencyInjectionRegister).Assembly);
            msc.AddOpenBehavior(typeof(CachingBehavior<,>));
            msc.AddOpenBehavior(typeof(ValidationBehavior<,>));
            msc.AddOpenBehavior(typeof(UnitOfWorkBehavior<,>));
        });
        services.AddValidatorsFromAssemblyContaining(typeof(DependencyInjectionRegister));
        // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddScoped<ICurrentUser, CurrentUser>();

        return services;
    }
}

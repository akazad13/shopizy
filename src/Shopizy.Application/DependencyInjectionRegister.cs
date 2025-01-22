using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shopizy.Application.Common.Behaviors;
using Shopizy.Application.Common.Caching;
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
            msc.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
            msc.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        services.AddValidatorsFromAssemblyContaining(typeof(DependencyInjectionRegister));
        // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.Configure<RedisSettings>(configuration.GetSection(RedisSettings.Section));

        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddSingleton<ICacheHelper, RedisCacheHelper>();

        return services;
    }
}

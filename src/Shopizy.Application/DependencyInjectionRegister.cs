using Shopizy.SharedKernel.Application.Messaging;
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
        services.Scan(scan => scan
            .FromAssemblies(typeof(DependencyInjectionRegister).Assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );

        // Decorators
        services.Decorate(typeof(ICommandHandler<,>), typeof(ValidationCommandHandlerDecorator<,>));
        services.Decorate(typeof(IQueryHandler<,>), typeof(ValidationQueryHandlerDecorator<,>));
        services.Decorate(typeof(ICommandHandler<,>), typeof(UnitOfWorkCommandHandlerDecorator<,>));
        services.Decorate(typeof(ICommandHandler<,>), typeof(CacheInvalidationCommandHandlerDecorator<,>));
        services.Decorate(typeof(IQueryHandler<,>), typeof(CachingQueryHandlerDecorator<,>));

        services.AddValidatorsFromAssemblyContaining(typeof(DependencyInjectionRegister));

        services.AddScoped<ICurrentUser, CurrentUser>();

        return services;
    }
}


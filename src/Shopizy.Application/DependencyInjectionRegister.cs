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
        // Note: CachingQueryHandlerDecorator requires ICachableRequest constraint which Scrutor Decorate might have issues with.
        // For simplicity, we might need a custom factory or explicit registration for cached queries if strictly required.

        services.AddValidatorsFromAssemblyContaining(typeof(DependencyInjectionRegister));

        services.AddScoped<ICurrentUser, CurrentUser>();

        return services;
    }
}


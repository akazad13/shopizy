using Microsoft.Extensions.DependencyInjection;
using Shopizy.Infrastructure.Messaging;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Infrastructure.DependencyInjection;

public static class MessagingRegister
{
    public static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        services.AddScoped<IDispatcher, Dispatcher>();

        return services;
    }
}

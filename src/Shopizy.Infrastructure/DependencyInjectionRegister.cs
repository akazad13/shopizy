using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shopizy.Infrastructure.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Shopizy.Infrastructure;

[ExcludeFromCodeCoverage]
public static class DependencyInjectionRegister
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddCors(options =>
        {
            options.AddPolicy(
                name: "_myAllowSpecificOrigins",
                builder =>
                {
                    builder
                        .SetIsOriginAllowed((host) => true)
                        .WithOrigins(Origins())
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                }
            );
        });

        return services
            .AddHttpContextAccessor()
            .AddExternalServices(configuration)
            .AddAuthenticationInternal(configuration)
            .AddSecurity()
            .AddPersistence(configuration);
    }

    private static string[] Origins()
    {
        return ["http://localhost:4200", "https://shopizy.netlify.app/"];
    }
}

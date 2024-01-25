using eStore.Application.Common.Interfaces;
using eStore.Infrastructure.Authentication;
using eStore.Infrastructure.Common.Persistence;
using eStore.Infrastructure.Security.CurrentUserProvider;
using eStore.Infrastructure.Security.TokenGenerator;
using eStore.Infrastructure.Security.TokenValidation;
using eStore.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eStore.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        ArgumentNullException.ThrowIfNull(services);
        return services
            .AddHttpContextAccessor()
            .AddServices()
            .AddBackgroundServices(configuration)
            .AddAuthentication(configuration)
            .AddAuthorization()
            .AddPersistence(configuration);
    }

    private static IServiceCollection AddBackgroundServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        return services;
    }

    public static IServiceCollection AddAuthorization(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

        return services;
    }

    public static IServiceCollection AddAuthentication(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.Section));

        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        services
            .ConfigureOptions<JwtBearerToeknValidationConfiguration>()
            .AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        return services;
    }

    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<AppDbContext>(
            options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
        );
        return services;
    }
}

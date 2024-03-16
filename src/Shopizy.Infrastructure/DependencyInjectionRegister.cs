using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Application.Common.Interfaces.Services;
using shopizy.Infrastructure.Products.Persistence;
using Shopizy.Infrastructure.Security;
using Shopizy.Infrastructure.Security.PolicyEnforcer;
using Shopizy.Application.Common.Interfaces.Authentication;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Infrastructure.Authentication;
using Shopizy.Infrastructure.Categories.Persistence;
using Shopizy.Infrastructure.Common.Persistence;
using Shopizy.Infrastructure.Customers.Persistence;
using Shopizy.Infrastructure.Orders.Persistence;
using Shopizy.Infrastructure.ProductReviews.Persistence;
using Shopizy.Infrastructure.PromoCodes.Persistence;
using Shopizy.Infrastructure.Security.CurrentUserProvider;
using Shopizy.Infrastructure.Security.Hashing;
using Shopizy.Infrastructure.Security.TokenGenerator;
using Shopizy.Infrastructure.Security.TokenValidation;
using Shopizy.Infrastructure.Services;
using Shopizy.Infrastructure.Users.Persistence;

namespace Shopizy.Infrastructure;

public static class DependencyInjectionRegister
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
            .AddPersistence(configuration)
            .AddRepositories();
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
        services.AddScoped<IAppDbContext, AppDbContext>();
        services.AddScoped<DbMigrationsHelper>();
        return services;
    }

    public static IServiceCollection AddAuthorization(this IServiceCollection services)
    {
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
        services.AddSingleton<IPolicyEnforcer, PolicyEnforcer>();

        return services;
    }

    public static IServiceCollection AddAuthentication(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.Section));

        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPasswordManager, PasswordManager>();

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

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IProductReviewRepository, ProductReviewRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IPromoCodeRepository, PromoCodeRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}

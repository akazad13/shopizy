using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using shopizy.Infrastructure.Products.Persistence;
using Shopizy.Application.Common.Interfaces.Authentication;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Infrastructure.Carts.Persistence;
using Shopizy.Infrastructure.Categories.Persistence;
using Shopizy.Infrastructure.Common.Persistence;
using Shopizy.Infrastructure.Customers.Persistence;
using Shopizy.Infrastructure.ExternalServices.MediaUploader.CloudinaryService;
using Shopizy.Infrastructure.ExternalServices.PaymentGateway.Stripe;
using Shopizy.Infrastructure.Orders.Persistence;
using Shopizy.Infrastructure.ProductReviews.Persistence;
using Shopizy.Infrastructure.PromoCodes.Persistence;
using Shopizy.Infrastructure.Security;
using Shopizy.Infrastructure.Security.CurrentUserProvider;
using Shopizy.Infrastructure.Security.Hashing;
using Shopizy.Infrastructure.Security.PolicyEnforcer;
using Shopizy.Infrastructure.Security.TokenGenerator;
using Shopizy.Infrastructure.Security.TokenValidation;
using Shopizy.Infrastructure.Services;
using Shopizy.Infrastructure.Users.Persistence;
using Stripe;
using Stripe.Checkout;

namespace Shopizy.Infrastructure;

public static class DependencyInjectionRegister
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        ArgumentNullException.ThrowIfNull(services);

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
            .AddServices(configuration)
            .AddBackgroundServices()
            .AddAuthentication(configuration)
            .AddAuthorization()
            .AddPersistence(configuration)
            .AddRepositories();
    }

    private static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {
        return services;
    }

    private static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        _ = services
            .AddSingleton<IDateTimeProvider, SystemDateTimeProvider>()
            .AddScoped<IAppDbContext, AppDbContext>()
            .AddScoped<DbMigrationsHelper>();

        _ = services.Configure<CloudinarySettings>(
            configuration.GetSection(CloudinarySettings.Section)
        );

        _ = services.AddTransient<ICloudinary, Cloudinary>(sp =>
        {
            var acc = new CloudinaryDotNet.Account(
                configuration.GetValue<string>("CloudinarySettings:CloudName"),
                configuration.GetValue<string>("CloudinarySettings:ApiKey"),
                configuration.GetValue<string>("CloudinarySettings:ApiSecret")
            );
            var cloudinary = new Cloudinary(acc);
            cloudinary.Api.Secure = configuration.GetValue<bool>("CloudinarySettings:Secure");
            return cloudinary;
        });
        _ = services
            .AddScoped<IMediaUploader, CloudinaryMediaUploader>()
            .AddScoped<IPaymentService, StripeService>()
            .AddScoped<TokenService>()
            .AddScoped<CustomerService>()
            .AddScoped<ChargeService>()
            .AddScoped<SessionService>();

        _ = services.Configure<StripeSettings>(configuration.GetSection(StripeSettings.Section));

        StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];

        return services;
    }

    public static IServiceCollection AddAuthorization(this IServiceCollection services)
    {
        _ = services
            .AddScoped<IAuthorizationService, AuthorizationService>()
            .AddScoped<ICurrentUserProvider, CurrentUserProvider>()
            .AddSingleton<IPolicyEnforcer, PolicyEnforcer>();

        return services;
    }

    public static IServiceCollection AddAuthentication(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        _ = services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.Section));

        _ = services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        _ = services.AddScoped<IPasswordManager, PasswordManager>();

        _ = services
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
        _ = services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
        );
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        _ = services
            .AddScoped<ICategoryRepository, CategoryRepository>()
            .AddScoped<ICartRepository, CartRepository>()
            .AddScoped<IOrderRepository, OrderRepository>()
            .AddScoped<IPaymentRepository, PaymentRepository>()
            .AddScoped<IProductReviewRepository, ProductReviewRepository>()
            .AddScoped<IProductRepository, ProductRepository>()
            .AddScoped<IPromoCodeRepository, PromoCodeRepository>()
            .AddScoped<IUserRepository, UserRepository>();

        return services;
    }

    private static string[] Origins()
    {
        return ["http://localhost:4200"];
    }
}

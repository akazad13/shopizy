using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shopizy.Application.Common.Interfaces.Authentication;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Common.Caching;
using StackExchange.Redis;
using Microsoft.Extensions.Options;
using Shopizy.Infrastructure.Carts.Persistence;
using Shopizy.Infrastructure.Categories.Persistence;
using Shopizy.Infrastructure.Common.Caching;
using Shopizy.Infrastructure.Common.Persistence;
using Shopizy.Infrastructure.Customers.Persistence;
using Shopizy.Infrastructure.ExternalServices.MediaUploader.CloudinaryService;
using Shopizy.Infrastructure.ExternalServices.PaymentGateway.Stripe;
using Shopizy.Infrastructure.Orders.Persistence;
using Shopizy.Infrastructure.Permissions.Persistence;
using Shopizy.Infrastructure.ProductReviews.Persistence;
using shopizy.Infrastructure.Products.Persistence;
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

namespace Shopizy.Infrastructure;

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
        services
            .AddSingleton<IDateTimeProvider, SystemDateTimeProvider>()
            .AddScoped<IAppDbContext, AppDbContext>()
            .AddScoped<DbMigrationsHelper>();

        services.Configure<CloudinarySettings>(
            configuration.GetSection(CloudinarySettings.Section)
        );

        services.AddTransient<ICloudinary, Cloudinary>(sp =>
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
        services
            .AddScoped<IMediaUploader, CloudinaryMediaUploader>()
            .AddScoped<IPaymentService, StripeService>()
            .AddScoped<CustomerService>()
            .AddScoped<PaymentIntentService>();

        services.Configure<StripeSettings>(configuration.GetSection(StripeSettings.Section));

        StripeConfiguration.ApiKey = configuration["StripeSettings:SecretKey"];

        services.Configure<RedisSettings>(configuration.GetSection(RedisSettings.Section));

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<RedisSettings>>().Value;
            var configurationOptions = new ConfigurationOptions
            {
                EndPoints = { { settings.Endpoint, settings.Port } },
                User = settings.Username,
                Password = settings.Password,
                AbortOnConnectFail = false
            };
            return ConnectionMultiplexer.Connect(configurationOptions);
        });

        services.AddSingleton<ICacheHelper, RedisCacheHelper>();

        return services;
    }

    public static IServiceCollection AddAuthorization(this IServiceCollection services)
    {
        services
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
        ArgumentNullException.ThrowIfNull(configuration);

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
        if (configuration.GetValue<bool>("UsePostgreSql"))
        {
            return services;
        }

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
        );
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services
            .AddScoped<ICategoryRepository, CategoryRepository>()
            .AddScoped<ICartRepository, CartRepository>()
            .AddScoped<IOrderRepository, OrderRepository>()
            .AddScoped<IPaymentRepository, PaymentRepository>()
            .AddScoped<IProductReviewRepository, ProductReviewRepository>()
            .AddScoped<IProductRepository, ProductRepository>()
            .AddScoped<IPromoCodeRepository, PromoCodeRepository>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IPermissionRepository, PermissionRepository>();

        return services;
    }

    private static string[] Origins()
    {
        return ["http://localhost:4200", "https://shopizy.netlify.app/"];
    }
}

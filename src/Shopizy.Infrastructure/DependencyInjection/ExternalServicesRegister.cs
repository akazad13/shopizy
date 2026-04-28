using CloudinaryDotNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shopizy.Application.Common.Interfaces.Authentication;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Infrastructure.Common.Caching;
using Shopizy.Infrastructure.Common.HealthChecks;
using Shopizy.Infrastructure.Common.Idempotency;
using Shopizy.Infrastructure.ExternalServices.MediaUploader.CloudinaryService;
using Shopizy.Infrastructure.ExternalServices.PaymentGateway.Stripe;
using Shopizy.Infrastructure.Security.RefreshTokens;
using Shopizy.Infrastructure.Services;
using Shopizy.SharedKernel.Application.Caching;
using StackExchange.Redis;
using Stripe;

namespace Shopizy.Infrastructure.DependencyInjection;

public static class ExternalServicesRegister
{
    public static IServiceCollection AddExternalServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Date Time Provider
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        // Cloudinary
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

        services.AddScoped<IMediaUploader, CloudinaryMediaUploader>();

        // Stripe — use a per-client API key instead of the global StripeConfiguration static.
        services.Configure<StripeSettings>(configuration.GetSection(StripeSettings.Section));

        services.AddSingleton<IStripeClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<StripeSettings>>().Value;
            return new StripeClient(apiKey: settings.SecretKey);
        });

        services
            .AddScoped<IPaymentService, StripeService>()
            .AddScoped(sp => new CustomerService(sp.GetRequiredService<IStripeClient>()))
            .AddScoped(sp => new PaymentIntentService(sp.GetRequiredService<IStripeClient>()))
            .AddScoped(sp => new RefundService(sp.GetRequiredService<IStripeClient>()));

        // Redis
        services.Configure<RedisSettings>(configuration.GetSection(RedisSettings.Section));

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<RedisSettings>>().Value;
            var configurationOptions = new ConfigurationOptions
            {
                EndPoints = { { settings.Endpoint, settings.Port } },
                User = settings.Username,
                Password = settings.Password,
                AbortOnConnectFail = false,
            };
            return ConnectionMultiplexer.Connect(configurationOptions);
        });

        services.AddSingleton<ICacheHelper, RedisCacheHelper>();
        services.AddSingleton<IIdempotencyStore, RedisIdempotencyStore>();

        services.Configure<RefreshTokenSettings>(
            configuration.GetSection(RefreshTokenSettings.Section)
        );
        services.AddSingleton<IRefreshTokenStore, RedisRefreshTokenStore>();
        services.AddSingleton<IRefreshTokenGenerator, RefreshTokenGenerator>();

        services.AddHealthChecks().AddCheck<RedisHealthCheck>("redis");

        services.AddScoped<IEmailService, LoggingEmailService>();

        return services;
    }
}

using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shopizy.Infrastructure.Common.Persistence;
using Shopizy.Infrastructure.Common.Persistence.Interceptors;
using Testcontainers.PostgreSql;
using Shopizy.SharedKernel.Application.Caching;
using Shopizy.SharedKernel.Application.Models;
using Shopizy.Application.Common.Interfaces.Services;

namespace Shopizy.Api.IntegrationTests;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder("postgres:15-alpine")
        .WithDatabase("shopizy_test")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Use environment variable to ensure it's available early for WebApplicationBuilder.Configuration
        Environment.SetEnvironmentVariable("UsePostgreSql", "true");
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["UsePostgreSql"] = "true",
                ["JwtSettings:Secret"] = "super-secret-key-that-is-at-least-32-chars-long",
                ["JwtSettings:Issuer"] = "Shopizy",
                ["JwtSettings:Audience"] = "Shopizy",
                ["JwtSettings:TokenExpirationMinutes"] = "60",
                ["RedisCacheSettings:Endpoint"] = "localhost",
                ["RedisCacheSettings:Port"] = "6379"
            });
        });

        builder.ConfigureTestServices(services =>
        {
            // Remove the existing DbContext registration (if any)
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));

            // Add the new DbContext registration using the Testcontainer
            // We use Npgsql here to match the PostgreSqlContainer
            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                var interceptor = sp.GetRequiredService<UpdateAuditableEntitiesInterceptor>();
                options.UseNpgsql(_dbContainer.GetConnectionString())
                    .AddInterceptors(interceptor);
            });

            // Override JWT validation for testing
            services.PostConfigure<Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerOptions>(
                Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme, 
                options =>
                {
                    options.TokenValidationParameters.ValidateIssuer = false;
                    options.TokenValidationParameters.ValidateAudience = false;
                    options.TokenValidationParameters.ValidateIssuerSigningKey = false;
                    options.RequireHttpsMetadata = false;
                });

            // Replace Redis cache with In-Memory stub
            services.RemoveAll(typeof(ICacheHelper));
            services.AddSingleton<ICacheHelper, InMemoryCacheHelper>();

            // Mock IPaymentService
            services.RemoveAll(typeof(IPaymentService));
            services.AddScoped<IPaymentService, MockPaymentService>();
        });
    }

    public class InMemoryCacheHelper : ICacheHelper
    {
        // No-op cache for integration tests - always returns cache miss
        // This ensures tests always get fresh data from the database
        
        public Task<CacheResult<T>> GetAsync<T>(string key)
        {
            // Always return cache miss
            return Task.FromResult(CacheResult<T>.Miss());
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            // Don't actually cache anything in tests
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            return Task.CompletedTask;
        }
    }

    public class MockPaymentService : IPaymentService
    {
        public Task<ErrorOr.ErrorOr<CustomerResource>> CreateCustomer(string email, string name, CancellationToken cancellationToken)
        {
            return Task.FromResult<ErrorOr.ErrorOr<CustomerResource>>(
                new CustomerResource("cus_mock_123", email, name)
            );
        }

        public Task<ErrorOr.ErrorOr<CreateSaleResponse>> CreateSaleAsync(CreateSaleRequest request)
        {
            return Task.FromResult<ErrorOr.ErrorOr<CreateSaleResponse>>(
                new CreateSaleResponse
                {
                    ResponseStatusCode = 200,
                    CustomerId = request.CustomerId,
                    Amount = request.Amount,
                    Currency = request.Currency,
                    PaymentIntentId = "pi_mock_123",
                    ObjectType = "payment_intent",
                    CaptureMethod = "automatic",
                    ChargeId = "ch_mock_123",
                    PaymentMethodId = request.PaymentMethodId,
                    PaymentMethodTypes = request.PaymentMethodTypes,
                    Status = "succeeded",
                    Metadata = new Dictionary<string, string>()
                }
            );
        }
    }

    public async ValueTask InitializeAsync()
    {
        await _dbContainer.StartAsync();

        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        try 
        {
            // We use EnsureCreatedAsync instead of MigrateAsync for tests because the migrations are SQL Server specific.
            // EnsureCreatedAsync will create the schema based on the current EF Core model, 
            // which Npgsql can translate to PostgreSQL.
            await dbContext.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"FATAL INITIALIZATION ERROR: {ex.Message}", ex);
        }
    }

    public new async ValueTask DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}

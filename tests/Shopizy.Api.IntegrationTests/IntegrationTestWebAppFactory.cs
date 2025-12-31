using Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shopizy.Infrastructure.Common.Persistence;
using Testcontainers.PostgreSql;
using Microsoft.Extensions.Configuration;

namespace Shopizy.Api.IntegrationTests;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:15-alpine")
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
                ["JwtSettings:ExpiryMinutes"] = "60",
                ["RedisSettings:Endpoint"] = "localhost",
                ["RedisSettings:Port"] = "6379"
            });
        });

        builder.ConfigureTestServices(services =>
        {
            // Remove the existing DbContext registration (if any)
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));

            // Add the new DbContext registration using the Testcontainer
            // We use Npgsql here to match the PostgreSqlContainer
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString());
            });
        });
    }

    public async Task InitializeAsync()
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

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}

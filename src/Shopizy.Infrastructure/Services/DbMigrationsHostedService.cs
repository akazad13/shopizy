using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Shopizy.Infrastructure.Services;

/// <summary>
/// Runs pending EF Core migrations once at startup. Using a hosted service keeps the entry point
/// free of scope plumbing and lets the host log/abort cleanly if migrations fail.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class DbMigrationsHostedService(
    IServiceScopeFactory scopeFactory,
    IHostEnvironment hostEnvironment
) : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly IHostEnvironment _hostEnvironment = hostEnvironment;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_hostEnvironment.IsEnvironment("Testing"))
        {
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var helper = scope.ServiceProvider.GetRequiredService<DbMigrationsHelper>();
        await helper.MigrateAsync().ConfigureAwait(false);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

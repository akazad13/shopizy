using Xunit;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shopizy.Infrastructure.Common.Persistence;

namespace Shopizy.Api.IntegrationTests;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>, IDisposable
{
    private readonly IServiceScope _scope;
    protected readonly ISender Sender;
    protected readonly AppDbContext DbContext;
    protected readonly HttpClient HttpClient;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        DbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        HttpClient = factory.CreateClient();
    }

    public void Dispose()
    {
        _scope.Dispose();
        DbContext.Dispose();
    }
}

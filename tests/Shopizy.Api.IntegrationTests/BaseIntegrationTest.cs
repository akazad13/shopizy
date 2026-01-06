using Xunit;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shopizy.Infrastructure.Common.Persistence;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using Shopizy.Contracts.Authentication;

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

    /// <summary>
    /// Registers a new user and returns the registration response.
    /// </summary>
    protected async Task<HttpResponseMessage> RegisterUserAsync(
        string firstName,
        string lastName,
        string email,
        string password)
    {
        var registerRequest = new RegisterRequest(firstName, lastName, email, password);
        return await HttpClient.PostAsJsonAsync("/api/v1.0/auth/register", registerRequest);
    }

    /// <summary>
    /// Logs in a user and returns the JWT token.
    /// </summary>
    protected async Task<string> LoginAndGetTokenAsync(string email, string password)
    {
        var loginRequest = new LoginRequest(email, password);
        var response = await HttpClient.PostAsJsonAsync("/api/v1.0/auth/login", loginRequest);
        response.EnsureSuccessStatusCode();

        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return authResponse?.Token ?? throw new InvalidOperationException("Failed to get auth token");
    }

    /// <summary>
    /// Sets the Authorization header with the provided JWT token.
    /// </summary>
    protected void SetAuthToken(string token)
    {
        HttpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);
    }

    /// <summary>
    /// Clears the Authorization header.
    /// </summary>
    protected void ClearAuthToken()
    {
        HttpClient.DefaultRequestHeaders.Authorization = null;
    }

    /// <summary>
    /// Registers a user, logs in, and sets the auth token in one call.
    /// </summary>
    protected async Task<string> AuthenticateAsNewUserAsync(
        string firstName = "Test",
        string lastName = "User",
        string? email = null,
        string password = "Password123!")
    {
        email ??= $"{Guid.NewGuid().ToString()[..8]}@test.com";
        
        await RegisterUserAsync(firstName, lastName, email, password);
        var token = await LoginAndGetTokenAsync(email, password);
        SetAuthToken(token);
        
        return token;
    }

    public void Dispose()
    {
        _scope.Dispose();
        DbContext.Dispose();
        HttpClient.Dispose();
    }
}

using System.Net;
using System.Net.Http.Json;
using Shopizy.Application.Auth.Commands.Register;
using Shopizy.Application.Auth.Queries.login;
using Shopizy.Contracts.Authentication;
using Shouldly;
using Xunit;

namespace Shopizy.Api.IntegrationTests.Auth;

public class LoginTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkAndToken()
    {
        // Arrange
        var email = $"{Guid.NewGuid().ToString()[..8]}@login.com";
        var password = "Password123!";
        var registerRequest = new RegisterCommand("Login", "User", email, password);
        await HttpClient.PostAsJsonAsync("/api/v1.0/auth/register", registerRequest);

        var loginRequest = new LoginQuery(email, password);

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1.0/auth/login", loginRequest);

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        response.StatusCode.ShouldBe(HttpStatusCode.OK, content);
        
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        authResponse.ShouldNotBeNull();
        authResponse.Token.ShouldNotBeNullOrEmpty();
        authResponse.Email.ShouldBe(email);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var email = $"{Guid.NewGuid().ToString()[..8]}@loginfailed.com";
        var password = "Password123!";
        var registerRequest = new RegisterCommand("Login", "User", email, password);
        await HttpClient.PostAsJsonAsync("/api/v1.0/auth/register", registerRequest);

        var loginRequest = new LoginQuery(email, "WrongPassword");

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1.0/auth/login", loginRequest);

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized, content);
    }
}

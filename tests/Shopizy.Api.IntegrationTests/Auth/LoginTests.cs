using System.Net;
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

        // Act
        var token = await AuthenticateAsNewUserAsync("Login", "User", email, password);

        // Assert
        token.ShouldNotBeNullOrEmpty();
        
        // Verify token works by making an authenticated request
        SetAuthToken(token);
        var response = await HttpClient.GetAsync("/api/v1.0/products", TestContext.Current.CancellationToken);
        
        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var email = $"{Guid.NewGuid().ToString()[..8]}@loginfailed.com";
        var password = "Password123!";
        await RegisterUserAsync("Login", "User", email, password);

        var loginRequest = new LoginRequest(email, "WrongPassword");

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1.0/auth/login", loginRequest, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}


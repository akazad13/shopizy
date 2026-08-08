using System.Net;
using System.Net.Http.Json;
using Shopizy.Contracts.Authentication;
using Shouldly;
using Xunit;

namespace Shopizy.Api.IntegrationTests.Auth;

public class AuthEndpointsIntegrationTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task ForgotPassword_WithRegisteredEmail_ShouldReturnOkWithToken()
    {
        // Arrange
        var email = $"forgot{Guid.NewGuid().ToString()[..6]}@test.com";
        await RegisterUserAsync("Forgot", "User", email, "Password123!");

        var request = new ForgotPasswordRequest(email);

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            "api/v1.0/auth/forgot-password",
            request,
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ForgotPasswordResponse>(
            TestContext.Current.CancellationToken
        );
        result.ShouldNotBeNull();
        result.Token.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task RefreshToken_WithInvalidToken_ShouldReturnBadRequestOrUnauthorized()
    {
        // Arrange
        var request = new RefreshTokenRequest("invalid_token_str");

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            "api/v1.0/auth/refresh",
            request,
            TestContext.Current.CancellationToken
        );

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
    }
}

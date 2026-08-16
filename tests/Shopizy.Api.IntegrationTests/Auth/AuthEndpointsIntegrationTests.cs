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
    public async Task PasswordReset_FullFlow_Succeeds()
    {
        // 1. Register User
        var email = $"resetflow{Guid.NewGuid().ToString()[..6]}@test.com";
        var oldPassword = "Password123!";
        var newPassword = "NewSecurePassword123!";
        await RegisterUserAsync("Reset", "Flow", email, oldPassword);

        // 2. Request Forgot Password Token
        var forgotRequest = new ForgotPasswordRequest(email);
        var forgotResponse = await HttpClient.PostAsJsonAsync(
            "api/v1.0/auth/forgot-password",
            forgotRequest,
            TestContext.Current.CancellationToken
        );
        forgotResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var forgotResult = await forgotResponse.Content.ReadFromJsonAsync<ForgotPasswordResponse>(
            TestContext.Current.CancellationToken
        );
        forgotResult.ShouldNotBeNull();

        // 3. Reset Password
        var resetRequest = new ResetPasswordRequest(forgotResult.Token, newPassword);
        var resetResponse = await HttpClient.PostAsJsonAsync(
            "api/v1.0/auth/reset-password",
            resetRequest,
            TestContext.Current.CancellationToken
        );
        resetResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        // 4. Login with New Password
        var loginRequest = new LoginRequest(email, newPassword);
        var loginResponse = await HttpClient.PostAsJsonAsync(
            "api/v1.0/auth/login",
            loginRequest,
            TestContext.Current.CancellationToken
        );
        loginResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ResetPassword_WithInvalidToken_ReturnsNotFoundOrBadRequest()
    {
        // Arrange
        var request = new ResetPasswordRequest("invalid_reset_token", "NewPassword123!");

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            "api/v1.0/auth/reset-password",
            request,
            TestContext.Current.CancellationToken
        );

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
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

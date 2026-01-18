using Xunit;
using System.Net;
using Shouldly;
using Shopizy.Application.Auth.Commands.Register;

namespace Shopizy.Api.IntegrationTests.Auth;

public class RegisterTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Register_WithValidRequest_ReturnsCreated()
    {
        // Arrange
        var request = new RegisterCommand(
            "Integration",
            "Test",
            $"{Guid.NewGuid().ToString()[..8]}@example.com",
            "Password123!"
        );

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1.0/auth/register", request);

        // Assert
        var content = await response.Content.ReadAsStringAsync();
        response.StatusCode.ShouldBe(HttpStatusCode.OK, $"Response content: {content}");
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsConflict()
    {
        // Arrange
        var email = $"{Guid.NewGuid().ToString()[..8]}@dup.com";
        await RegisterUserAsync("First", "User", email, "Password123!");

        // Act
        var response = await RegisterUserAsync("Second", "User", email, "Password123!");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Register_WithInvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var request = new RegisterCommand("Invalid", "Email", "not-an-email", "Password123!");

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1.0/auth/register", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}

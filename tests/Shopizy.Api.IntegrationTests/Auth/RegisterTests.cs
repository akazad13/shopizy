using Xunit;
using System.Net;
using System.Net.Http.Json;
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
            $"test_{Guid.NewGuid()}@example.com",
            "Password123!"
        );

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1.0/auth/register", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsConflict()
    {
        // Arrange
        var email = $"duplicate_{Guid.NewGuid()}@example.com";
        var request = new RegisterCommand("First", "User", email, "Password123!");
        
        await HttpClient.PostAsJsonAsync("/api/v1.0/auth/register", request);

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1.0/auth/register", request);

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

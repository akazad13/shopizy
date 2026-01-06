using System.Net;
using System.Net.Http.Json;
using Shouldly;
using Xunit;

namespace Shopizy.Api.IntegrationTests.Auth;

public class AuthorizationTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetUser_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var userId = Guid.NewGuid();
        ClearAuthToken(); // Ensure no auth token

        // Act
        var response = await HttpClient.GetAsync($"/api/v1.0/users/{userId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCart_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var userId = Guid.NewGuid();
        ClearAuthToken();

        // Act
        var response = await HttpClient.GetAsync($"/api/v1.0/users/{userId}/carts");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateOrder_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var orderRequest = new { CartId = Guid.NewGuid() };
        ClearAuthToken();

        // Act
        var response = await HttpClient.PostAsJsonAsync($"/api/v1.0/users/{userId}/orders", orderRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ProcessPayment_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var paymentRequest = new { OrderId = Guid.NewGuid(), PaymentMethod = "card" };
        ClearAuthToken();

        // Act
        var response = await HttpClient.PostAsJsonAsync($"/api/v1.0/users/{userId}/payments", paymentRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task BrowseProducts_WithoutAuthentication_ReturnsSuccess()
    {
        // Arrange
        ClearAuthToken(); // Ensure no auth - public endpoint

        // Act
        var response = await HttpClient.GetAsync("/api/v1.0/products");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task BrowseCategories_WithoutAuthentication_ReturnsSuccess()
    {
        // Arrange
        ClearAuthToken(); // Public endpoint

        // Act
        var response = await HttpClient.GetAsync("/api/v1.0/categories");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
    [Fact]
    public async Task AuthenticatedRequest_WithValidToken_ReturnsSuccess()
    {
        // Arrange
        var email = $"{Guid.NewGuid().ToString()[..8]}@auth.com";
        await AuthenticateAsNewUserAsync("Auth", "User", email, "Password123!");

        // Act
        var response = await HttpClient.GetAsync("/api/v1.0/products");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateProduct_AsRegularUser_ReturnsForbidden()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = $"{Guid.NewGuid().ToString()[..8]}@regular.com";
        await AuthenticateAsNewUserAsync("Regular", "User", email, "Password123!");
        
        var productRequest = new { Name = "New Product", Price = 10.0 };

        // Act
        var response = await HttpClient.PostAsJsonAsync($"/api/v1.0/users/{userId}/products", productRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}

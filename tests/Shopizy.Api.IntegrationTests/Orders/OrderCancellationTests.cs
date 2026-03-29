using System.Net;
using Shopizy.Contracts.Category;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Order;
using Shopizy.Contracts.Product;
using Shouldly;
using Xunit;

namespace Shopizy.Api.IntegrationTests.Orders;

public class OrderCancellationTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    private async Task<(Guid UserId, Guid OrderId)> SetupOrderAsync()
    {
        await AuthenticateAsAdminAsync();

        var catResponse = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/admin/categories",
            new CreateCategoryRequest($"CancelCat {Guid.NewGuid().ToString()[..4]}", null),
            TestContext.Current.CancellationToken);
        catResponse.EnsureSuccessStatusCode();
        var category = await catResponse.Content.ReadFromJsonAsync<CategoryResponse>(TestContext.Current.CancellationToken);

        var prodResponse = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/admin/products",
            new CreateProductRequest(
                $"CancelProd {Guid.NewGuid().ToString()[..4]}", "Short", "Full", category!.Id,
                49.99m, 1, 0m, $"CXL-{Guid.NewGuid().ToString()[..6]}", "CancelBrand",
                "Black", "S", "cancel", Guid.NewGuid().ToString()[..8], 100, null),
            TestContext.Current.CancellationToken);
        prodResponse.EnsureSuccessStatusCode();
        var product = await prodResponse.Content.ReadFromJsonAsync<ProductResponse>(TestContext.Current.CancellationToken);

        var (_, userId) = await AuthenticateAsNewUserAsync("Cancel", "Customer");

        var orderResponse = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/orders/checkout",
            new CreateOrderRequest("", 1, new Price(0m, "USD"),
                [new OrderItemRequest(product!.ProductId, "Black", "S", 1)],
                new Address("1 Cancel St", "City", "ST", "Country", "00001")),
            TestContext.Current.CancellationToken);
        orderResponse.EnsureSuccessStatusCode();
        var order = await orderResponse.Content.ReadFromJsonAsync<OrderDetailResponse>(TestContext.Current.CancellationToken);

        return (userId, order!.OrderId);
    }

    [Fact]
    public async Task CancelOrder_WithValidOrder_ReturnsOk()
    {
        // Arrange
        var (userId, orderId) = await SetupOrderAsync();

        // Act
        var response = await HttpClient.PatchAsJsonAsync(
            $"/api/v1.0/users/{userId}/orders/{orderId}/cancel",
            new CancelOrderRequest("Changed my mind"),
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<SuccessResult>(TestContext.Current.CancellationToken);
        result.ShouldNotBeNull();
        result.Message.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task CancelOrder_WithWrongUserIdInRoute_ReturnsForbidden()
    {
        // Arrange — SetupOrderAsync leaves us authenticated as the order owner
        var (_, orderId) = await SetupOrderAsync();
        var anotherUserId = Guid.NewGuid(); // a random userId not matching the JWT

        // Act — IsAuthorized check fails because anotherUserId != JWT subject
        var response = await HttpClient.PatchAsJsonAsync(
            $"/api/v1.0/users/{anotherUserId}/orders/{orderId}/cancel",
            new CancelOrderRequest("Wrong user in route"),
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CancelOrder_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthToken();

        // Act
        var response = await HttpClient.PatchAsJsonAsync(
            $"/api/v1.0/users/{Guid.NewGuid()}/orders/{Guid.NewGuid()}/cancel",
            new CancelOrderRequest("No auth"),
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CancelOrder_WithNonExistentOrderId_ReturnsNotFound()
    {
        // Arrange
        var (_, _) = await SetupOrderAsync(); // sets up auth as a customer
        var token = HttpClient.DefaultRequestHeaders.Authorization!.Parameter!;
        var userId = GetUserIdFromToken(token);

        // Act — use a random orderId that doesn't exist
        var response = await HttpClient.PatchAsJsonAsync(
            $"/api/v1.0/users/{userId}/orders/{Guid.NewGuid()}/cancel",
            new CancelOrderRequest("No such order"),
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}

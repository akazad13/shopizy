using System.Net;
using Shopizy.Contracts.Category;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Order;
using Shopizy.Contracts.Product;
using Shouldly;
using Xunit;

namespace Shopizy.Api.IntegrationTests.Admin;

public class AdminOrderTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    private async Task<(Guid ProductId, Guid OrderId, Guid CustomerId)> SetupOrderAsync()
    {
        // Admin creates category + product
        await AuthenticateAsAdminAsync();

        var catResponse = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/admin/categories",
            new CreateCategoryRequest($"AdminOrd Cat {Guid.NewGuid().ToString()[..4]}", null),
            TestContext.Current.CancellationToken);
        catResponse.EnsureSuccessStatusCode();
        var category = await catResponse.Content.ReadFromJsonAsync<CategoryResponse>(TestContext.Current.CancellationToken);

        var prodResponse = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/admin/products",
            new CreateProductRequest(
                $"AdminOrd Prod {Guid.NewGuid().ToString()[..4]}", "Short", "Full", category!.Id,
                99.00m, 1, 0m, $"AO-{Guid.NewGuid().ToString()[..6]}", "AO Brand",
                "Red", "M", "admin,order", Guid.NewGuid().ToString()[..8], 100, null),
            TestContext.Current.CancellationToken);
        prodResponse.EnsureSuccessStatusCode();
        var product = await prodResponse.Content.ReadFromJsonAsync<ProductResponse>(TestContext.Current.CancellationToken);

        // Customer places order
        var (_, customerId) = await AuthenticateAsNewUserAsync("AO", "Customer");
        var orderResponse = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/orders/checkout",
            new CreateOrderRequest("", 1, new Price(5.00m, "USD"),
                [new OrderItemRequest(product!.ProductId, "Red", "M", 1)],
                new Address("1 Test St", "City", "ST", "Country", "11111")),
            TestContext.Current.CancellationToken);
        orderResponse.EnsureSuccessStatusCode();
        var order = await orderResponse.Content.ReadFromJsonAsync<OrderDetailResponse>(TestContext.Current.CancellationToken);

        // Re-authenticate as admin for the test assertions
        await AuthenticateAsAdminAsync();

        return (product.ProductId, order!.OrderId, customerId);
    }

    [Fact]
    public async Task GetAdminOrders_AsAdmin_ReturnsOkWithList()
    {
        // Arrange
        await SetupOrderAsync();

        // Act
        var response = await HttpClient.GetAsync(
            "/api/v1.0/admin/orders?pageNumber=1&pageSize=10", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var orders = await response.Content.ReadFromJsonAsync<List<OrderResponse>>(TestContext.Current.CancellationToken);
        orders.ShouldNotBeNull();
        orders.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task GetAdminOrders_AsCustomer_ReturnsForbidden()
    {
        // Arrange
        await AuthenticateAsNewUserAsync("AO", "ForbiddenUser");

        // Act
        var response = await HttpClient.GetAsync(
            "/api/v1.0/admin/orders?pageNumber=1&pageSize=10", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAdminOrders_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthToken();

        // Act
        var response = await HttpClient.GetAsync(
            "/api/v1.0/admin/orders?pageNumber=1&pageSize=10", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAdminOrder_WithValidId_ReturnsOrderDetails()
    {
        // Arrange
        var (_, orderId, _) = await SetupOrderAsync();

        // Act
        var response = await HttpClient.GetAsync(
            $"/api/v1.0/admin/orders/{orderId}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var order = await response.Content.ReadFromJsonAsync<OrderDetailResponse>(TestContext.Current.CancellationToken);
        order.ShouldNotBeNull();
        order.OrderId.ShouldBe(orderId);
    }

    [Fact]
    public async Task GetAdminOrder_AsCustomer_ReturnsForbidden()
    {
        // Arrange
        await AuthenticateAsNewUserAsync("GetOrd", "ForbiddenCustomer");

        // Act
        var response = await HttpClient.GetAsync(
            $"/api/v1.0/admin/orders/{Guid.NewGuid()}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateOrderStatus_AsAdmin_ReturnsOk()
    {
        // Arrange
        var (_, orderId, _) = await SetupOrderAsync();

        // Act — mark as Processing (2)
        var response = await HttpClient.PatchAsJsonAsync(
            $"/api/v1.0/admin/orders/{orderId}/status",
            OrderStatus.Processing,
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<SuccessResult>(TestContext.Current.CancellationToken);
        result.ShouldNotBeNull();
        result.Message.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task UpdateOrderStatus_ToShipped_ReturnsOk()
    {
        // Arrange
        var (_, orderId, _) = await SetupOrderAsync();

        // Act — mark as Shipped (3)
        var response = await HttpClient.PatchAsJsonAsync(
            $"/api/v1.0/admin/orders/{orderId}/status",
            OrderStatus.Shipped,
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateOrderStatus_AsCustomer_ReturnsForbidden()
    {
        // Arrange
        await AuthenticateAsNewUserAsync("StatusCust", "User");

        // Act
        var response = await HttpClient.PatchAsJsonAsync(
            $"/api/v1.0/admin/orders/{Guid.NewGuid()}/status",
            OrderStatus.Cancelled,
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAdminOrders_WithStatusFilter_ReturnsFilteredResults()
    {
        // Arrange
        await SetupOrderAsync();

        // Act — filter by Pending status (1)
        var response = await HttpClient.GetAsync(
            "/api/v1.0/admin/orders?pageNumber=1&pageSize=20&status=1", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var orders = await response.Content.ReadFromJsonAsync<List<OrderResponse>>(TestContext.Current.CancellationToken);
        orders.ShouldNotBeNull();
        // All returned orders should have Pending status
        orders.All(o => o.OrderStatus == "Pending").ShouldBeTrue();
    }
}

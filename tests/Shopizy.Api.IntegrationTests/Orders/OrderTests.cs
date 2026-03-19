using System.Net;
using Shopizy.Contracts.Category;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Order;
using Shopizy.Contracts.Product;
using Shouldly;
using Xunit;

namespace Shopizy.Api.IntegrationTests.Orders;

public class OrderTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    // --- Setup helpers ---

    private async Task<(Guid CategoryId, Guid ProductId)> SetupProductAsync()
    {
        await AuthenticateAsAdminAsync();

        var catResponse = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/admin/categories",
            new CreateCategoryRequest($"Order Cat {Guid.NewGuid().ToString()[..4]}", null),
            TestContext.Current.CancellationToken);
        catResponse.EnsureSuccessStatusCode();
        var category = await catResponse.Content.ReadFromJsonAsync<CategoryResponse>(TestContext.Current.CancellationToken);

        var prodResponse = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/admin/products",
            new CreateProductRequest(
                $"Order Product {Guid.NewGuid().ToString()[..4]}", "Short", "Full desc", category!.Id,
                75.00m, 1, 0m, $"ORD-{Guid.NewGuid().ToString()[..6]}", "OrderBrand",
                "Blue", "M", "order", Guid.NewGuid().ToString()[..8], 200, null),
            TestContext.Current.CancellationToken);
        prodResponse.EnsureSuccessStatusCode();
        var product = await prodResponse.Content.ReadFromJsonAsync<ProductResponse>(TestContext.Current.CancellationToken);

        return (category.Id, product!.ProductId);
    }

    private async Task<Guid> PlaceOrderAsync(Guid productId)
    {
        var orderRequest = new CreateOrderRequest(
            PromoCode: "",
            DeliveryMethod: 1,
            DeliveryCharge: new Price(5.00m, "USD"),
            OrderItems: [new OrderItemRequest(productId, "Blue", "M", 1)],
            ShippingAddress: new Address("1 Test St", "Test City", "TS", "Test Country", "12345")
        );

        var response = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/orders/checkout", orderRequest, TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        var order = await response.Content.ReadFromJsonAsync<OrderDetailResponse>(TestContext.Current.CancellationToken);
        return order!.OrderId;
    }

    // --- Tests ---

    [Fact]
    public async Task CreateOrder_WithValidItems_ReturnsOkWithOrder()
    {
        // Arrange
        var (_, productId) = await SetupProductAsync();
        var (_, userId) = await AuthenticateAsNewUserAsync("Order", "Customer");

        var orderRequest = new CreateOrderRequest(
            PromoCode: "",
            DeliveryMethod: 1,
            DeliveryCharge: new Price(5.00m, "USD"),
            OrderItems: [new OrderItemRequest(productId, "Blue", "M", 2)],
            ShippingAddress: new Address("42 Commerce Rd", "Capital City", "CC", "Testland", "99999")
        );

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/orders/checkout", orderRequest, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var order = await response.Content.ReadFromJsonAsync<OrderDetailResponse>(TestContext.Current.CancellationToken);
        order.ShouldNotBeNull();
        order.UserId.ShouldBe(userId);
        order.OrderItems.ShouldNotBeEmpty();
        order.OrderItems.First().Name.ShouldNotBeNullOrEmpty();
        order.OrderStatus.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task CreateOrder_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthToken();
        var orderRequest = new CreateOrderRequest(
            "", 1, new Price(0m, "USD"),
            [new OrderItemRequest(Guid.NewGuid(), "Red", "S", 1)],
            new Address("1 St", "City", "ST", "Country", "00000"));

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/orders/checkout", orderRequest, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetOrders_WhenAuthenticated_ReturnsOkWithList()
    {
        // Arrange
        var (_, productId) = await SetupProductAsync();
        var (_, userId) = await AuthenticateAsNewUserAsync("ListOrder", "Customer");
        await PlaceOrderAsync(productId);

        // Act
        var response = await HttpClient.GetAsync(
            $"/api/v1.0/users/{userId}/orders", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var orders = await response.Content.ReadFromJsonAsync<List<OrderResponse>>(TestContext.Current.CancellationToken);
        orders.ShouldNotBeNull();
        orders.ShouldNotBeEmpty();
        orders.All(o => o.UserId == userId).ShouldBeTrue();
    }

    [Fact]
    public async Task GetOrders_ForAnotherUser_ReturnsForbidden()
    {
        // Arrange
        var (_, user1Id) = await AuthenticateAsNewUserAsync("OrdA", "User");
        var (_, user2Id) = await AuthenticateAsNewUserAsync("OrdB", "User");
        // Authenticated as user2; try to list user1's orders

        // Act
        var response = await HttpClient.GetAsync(
            $"/api/v1.0/users/{user1Id}/orders", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetOrders_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthToken();

        // Act
        var response = await HttpClient.GetAsync(
            $"/api/v1.0/users/{Guid.NewGuid()}/orders", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetOrder_WhenAuthenticated_ReturnsOrderDetails()
    {
        // Arrange
        var (_, productId) = await SetupProductAsync();
        var (_, userId) = await AuthenticateAsNewUserAsync("GetOrd", "Customer");
        var orderId = await PlaceOrderAsync(productId);

        // Act
        var response = await HttpClient.GetAsync(
            $"/api/v1.0/users/{userId}/orders/{orderId}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var order = await response.Content.ReadFromJsonAsync<OrderDetailResponse>(TestContext.Current.CancellationToken);
        order.ShouldNotBeNull();
        order.OrderId.ShouldBe(orderId);
        order.UserId.ShouldBe(userId);
    }

    [Fact]
    public async Task GetOrder_ForAnotherUser_ReturnsForbidden()
    {
        // Arrange
        var (_, user1Id) = await AuthenticateAsNewUserAsync("OrdGetA", "User");
        var (_, user2Id) = await AuthenticateAsNewUserAsync("OrdGetB", "User");
        // Authenticated as user2; try to get user1's specific order

        // Act
        var response = await HttpClient.GetAsync(
            $"/api/v1.0/users/{user1Id}/orders/{Guid.NewGuid()}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetOrders_WithStatusFilter_ReturnsFilteredResults()
    {
        // Arrange
        var (_, productId) = await SetupProductAsync();
        var (_, userId) = await AuthenticateAsNewUserAsync("StatusFilter", "Customer");
        await PlaceOrderAsync(productId);

        // Act — filter by Pending status
        var response = await HttpClient.GetAsync(
            $"/api/v1.0/users/{userId}/orders?status=1", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var orders = await response.Content.ReadFromJsonAsync<List<OrderResponse>>(TestContext.Current.CancellationToken);
        orders.ShouldNotBeNull();
    }

    [Fact]
    public async Task ProcessPayment_ForOrder_ReturnsOk()
    {
        // Arrange
        var (_, productId) = await SetupProductAsync();
        var (_, userId) = await AuthenticateAsNewUserAsync("PayOrd", "Customer");
        var orderId = await PlaceOrderAsync(productId);

        // Get order details to obtain item price
        var orderResponse = await HttpClient.GetAsync(
            $"/api/v1.0/users/{userId}/orders/{orderId}", TestContext.Current.CancellationToken);
        var order = await orderResponse.Content.ReadFromJsonAsync<OrderDetailResponse>(TestContext.Current.CancellationToken);

        var paymentRequest = new Shopizy.Contracts.Payment.CardNotPresentSaleRequest(
            orderId,
            order!.OrderItems.First().UnitPrice.Amount,
            order.OrderItems.First().UnitPrice.Currency,
            "card",
            "pm_card_visa",
            null
        );

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            $"/api/v1.0/users/{userId}/payments", paymentRequest, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<SuccessResult>(TestContext.Current.CancellationToken);
        result.ShouldNotBeNull();
        result.Message.ShouldNotBeNullOrEmpty();
    }
}

using System.Net;
using System.Net.Http.Headers;
using Shopizy.Contracts.Category;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Order;
using Shopizy.Contracts.Product;
using Shouldly;
using Xunit;

namespace Shopizy.Api.IntegrationTests.Auth;

/// <summary>
/// Asserts users cannot access another user's resources via path-bound user IDs.
/// </summary>
public class CrossTenantAuthorizationTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    private async Task<Guid> SetupProductAsync()
    {
        await AuthenticateAsAdminAsync();

        var catResponse = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/admin/categories",
            new CreateCategoryRequest($"AuthZ Cat {Guid.NewGuid().ToString()[..4]}", null),
            TestContext.Current.CancellationToken);
        catResponse.EnsureSuccessStatusCode();
        var category = await catResponse.Content.ReadFromJsonAsync<CategoryResponse>(TestContext.Current.CancellationToken);

        var prodResponse = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/admin/products",
            new CreateProductRequest(
                $"AuthZ Prod {Guid.NewGuid().ToString()[..4]}", "Short", "Full desc", category!.Id,
                10.00m, 1, 0m, $"AUTHZ-{Guid.NewGuid().ToString()[..6]}", null,
                "Red", "S", "authz", Guid.NewGuid().ToString()[..8], 50, null),
            TestContext.Current.CancellationToken);
        prodResponse.EnsureSuccessStatusCode();
        var product = await prodResponse.Content.ReadFromJsonAsync<ProductResponse>(TestContext.Current.CancellationToken);
        return product!.ProductId;
    }

    private async Task<Guid> PlaceOrderForCurrentUserAsync(Guid productId)
    {
        var orderRequest = new CreateOrderRequest(
            PromoCode: "",
            DeliveryMethod: 1,
            DeliveryCharge: new Price(5.00m, "USD"),
            OrderItems: [new OrderItemRequest(productId, "Red", "S", 1)],
            ShippingAddress: new Address("1 Test", "City", "TS", "Country", "12345")
        );

        var response = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/orders/checkout", orderRequest, TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        var order = await response.Content.ReadFromJsonAsync<OrderDetailResponse>(TestContext.Current.CancellationToken);
        return order!.OrderId;
    }

    [Fact]
    public async Task GetOrder_AsDifferentUser_ReturnsForbidden()
    {
        var productId = await SetupProductAsync();

        // User A places an order
        var (_, userAId) = await AuthenticateAsNewUserAsync("AuthZ", "OwnerA");
        var orderId = await PlaceOrderForCurrentUserAsync(productId);

        // User B tries to read User A's order
        await AuthenticateAsNewUserAsync("AuthZ", "AttackerB");

        var response = await HttpClient.GetAsync(
            $"/api/v1.0/users/{userAId}/orders/{orderId}", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ListOrders_AsDifferentUser_ReturnsForbidden()
    {
        var (_, userAId) = await AuthenticateAsNewUserAsync("AuthZ", "OwnerList");
        await AuthenticateAsNewUserAsync("AuthZ", "AttackerList");

        var response = await HttpClient.GetAsync(
            $"/api/v1.0/users/{userAId}/orders", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetCart_AsDifferentUser_ReturnsForbidden()
    {
        var (_, userAId) = await AuthenticateAsNewUserAsync("AuthZ", "OwnerCart");
        await AuthenticateAsNewUserAsync("AuthZ", "AttackerCart");

        var response = await HttpClient.GetAsync(
            $"/api/v1.0/users/{userAId}/cart", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ProcessPayment_AsDifferentUser_ReturnsForbidden()
    {
        var (_, userAId) = await AuthenticateAsNewUserAsync("AuthZ", "OwnerPay");
        await AuthenticateAsNewUserAsync("AuthZ", "AttackerPay");

        var paymentRequest = new
        {
            orderId = Guid.NewGuid(),
            paymentMethod = "card",
            amount = 10m,
            currency = "USD",
            token = "tok_visa"
        };

        var response = await HttpClient.PostAsJsonAsync(
            $"/api/v1.0/users/{userAId}/payments", paymentRequest, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}

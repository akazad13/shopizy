using System.Net;
using System.Net.Http.Headers;
using Shopizy.Contracts.Category;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Order;
using Shopizy.Contracts.Product;
using Shouldly;
using Xunit;

namespace Shopizy.Api.IntegrationTests.Orders;

public class OrderIdempotencyTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    private const string OrderEndpoint = "/api/v1.0/orders/checkout";

    private async Task<Guid> SetupProductAsync()
    {
        await AuthenticateAsAdminAsync();

        var catResponse = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/admin/categories",
            new CreateCategoryRequest($"Idem Cat {Guid.NewGuid().ToString()[..4]}", null),
            TestContext.Current.CancellationToken
        );
        catResponse.EnsureSuccessStatusCode();
        var category = await catResponse.Content.ReadFromJsonAsync<CategoryResponse>(
            TestContext.Current.CancellationToken
        );

        var prodResponse = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/admin/products",
            new CreateProductRequest(
                $"Idem Product {Guid.NewGuid().ToString()[..4]}",
                "Short",
                "Full desc",
                category!.Id,
                75.00m,
                1,
                0m,
                $"IDEM-{Guid.NewGuid().ToString()[..6]}",
                null,
                "Blue",
                "M",
                "idem",
                Guid.NewGuid().ToString()[..8],
                200,
                null
            ),
            TestContext.Current.CancellationToken
        );
        prodResponse.EnsureSuccessStatusCode();
        var product = await prodResponse.Content.ReadFromJsonAsync<ProductResponse>(
            TestContext.Current.CancellationToken
        );
        return product!.ProductId;
    }

    private static CreateOrderRequest BuildOrder(Guid productId, int quantity = 1) =>
        new(
            PromoCode: "",
            DeliveryMethod: 1,
            DeliveryCharge: new Price(5.00m, "USD"),
            OrderItems: [new OrderItemRequest(productId, "Blue", "M", quantity)],
            ShippingAddress: new Address("1 Test St", "Test City", "TS", "Test Country", "12345")
        );

    private async Task<HttpResponseMessage> PostOrderAsync(
        CreateOrderRequest order,
        string? idempotencyKey
    )
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, OrderEndpoint)
        {
            Content = JsonContent.Create(order),
        };
        request.Headers.Authorization = HttpClient.DefaultRequestHeaders.Authorization;
        if (idempotencyKey is not null)
        {
            request.Headers.TryAddWithoutValidation("Idempotency-Key", idempotencyKey);
        }
        return await HttpClient.SendAsync(request, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task CreateOrder_WithoutIdempotencyKey_Returns400()
    {
        var productId = await SetupProductAsync();
        await AuthenticateAsNewUserAsync("Idem", "NoKey");

        // The default HttpClient header sets a key — clear it for this test only.
        HttpClient.DefaultRequestHeaders.Remove("Idempotency-Key");

        var response = await PostOrderAsync(BuildOrder(productId), idempotencyKey: null);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOrder_SameKeyAndBody_ReplaysSameResponse()
    {
        var productId = await SetupProductAsync();
        await AuthenticateAsNewUserAsync("Idem", "Replay");

        // Make sure the request sent here has the explicit key, not the per-test default
        HttpClient.DefaultRequestHeaders.Remove("Idempotency-Key");

        var key = Guid.NewGuid().ToString("N");
        var order = BuildOrder(productId);

        var first = await PostOrderAsync(order, idempotencyKey: key);
        first.StatusCode.ShouldBe(HttpStatusCode.OK);
        var firstBody = await first.Content.ReadAsStringAsync(
            TestContext.Current.CancellationToken
        );

        var second = await PostOrderAsync(order, idempotencyKey: key);
        second.StatusCode.ShouldBe(HttpStatusCode.OK);
        var secondBody = await second.Content.ReadAsStringAsync(
            TestContext.Current.CancellationToken
        );

        secondBody.ShouldBe(firstBody);
    }

    [Fact]
    public async Task CreateOrder_SameKeyDifferentBody_Returns409()
    {
        var productId = await SetupProductAsync();
        await AuthenticateAsNewUserAsync("Idem", "Conflict");

        HttpClient.DefaultRequestHeaders.Remove("Idempotency-Key");

        var key = Guid.NewGuid().ToString("N");
        var first = await PostOrderAsync(BuildOrder(productId, quantity: 1), idempotencyKey: key);
        first.StatusCode.ShouldBe(HttpStatusCode.OK);

        var second = await PostOrderAsync(BuildOrder(productId, quantity: 5), idempotencyKey: key);
        second.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }
}

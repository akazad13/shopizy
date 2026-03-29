using System.Net;
using Shopizy.Contracts.Cart;
using Shopizy.Contracts.Category;
using Shopizy.Contracts.Order;
using Shopizy.Contracts.Payment;
using Shopizy.Contracts.Product;
using Shouldly;
using Xunit;

namespace Shopizy.Api.IntegrationTests.Workflows;

public class OrderWorkflowTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    // --- Setup helpers ---

    private async Task<(Guid CategoryId, Guid ProductId)> SetupProductAsync(int stockQuantity = 50)
    {
        await AuthenticateAsAdminAsync();

        var catResponse = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/admin/categories",
            new CreateCategoryRequest($"WF Cat {Guid.NewGuid().ToString()[..4]}", null),
            TestContext.Current.CancellationToken);
        catResponse.EnsureSuccessStatusCode();
        var category = await catResponse.Content.ReadFromJsonAsync<CategoryResponse>(TestContext.Current.CancellationToken);

        var prodResponse = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/admin/products",
            new CreateProductRequest(
                $"WF Product {Guid.NewGuid().ToString()[..4]}", "Short", "Full desc", category!.Id,
                50.00m, 1, 0m, $"WF-{Guid.NewGuid().ToString()[..6]}", "WFBrand",
                "Black", "L", "wf", Guid.NewGuid().ToString()[..8], stockQuantity, null),
            TestContext.Current.CancellationToken);
        prodResponse.EnsureSuccessStatusCode();
        var product = await prodResponse.Content.ReadFromJsonAsync<ProductResponse>(TestContext.Current.CancellationToken);

        return (category.Id, product!.ProductId);
    }

    // --- Test: Cart is cleared after placing an order ---

    [Fact]
    public async Task PlaceOrder_CartIsCleared()
    {
        // Arrange — admin creates product
        var (_, productId) = await SetupProductAsync();

        // Authenticate as customer
        var (_, userId) = await AuthenticateAsNewUserAsync("Cart", "Clearer");

        // Add item to cart
        var addResponse = await HttpClient.PatchAsJsonAsync(
            $"/api/v1.0/users/{userId}/cart/items",
            new AddProductToCartRequest(productId, "Black", "L", 1),
            TestContext.Current.CancellationToken);
        addResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        // Verify cart has the item before placing order
        var cartBefore = await GetCartAsync(userId);
        cartBefore.CartItems.ShouldNotBeEmpty();
        cartBefore.CartItems.ShouldContain(i => i.ProductId == productId);

        // Act — place order
        var orderRequest = new CreateOrderRequest(
            PromoCode: "",
            DeliveryMethod: 1,
            DeliveryCharge: new Price(5.00m, "USD"),
            OrderItems: [new OrderItemRequest(productId, "Black", "L", 1)],
            ShippingAddress: new Address("1 Workflow St", "Testville", "TV", "Testland", "00001")
        );
        var placeResponse = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/orders/checkout", orderRequest, TestContext.Current.CancellationToken);
        placeResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        // Assert — cart should be empty after order is placed
        // EventualConsistencyMiddleware processes OrderCreatedDomainEvent synchronously before returning
        var cartAfter = await GetCartAsync(userId);
        cartAfter.CartItems.ShouldBeEmpty();
    }

    // --- Test: Payment success transitions order to Processing ---

    [Fact]
    public async Task ProcessPayment_OrderStatusBecomesProcessing()
    {
        // Arrange — admin creates product
        var (_, productId) = await SetupProductAsync();

        // Authenticate as customer
        var (_, userId) = await AuthenticateAsNewUserAsync("Payment", "Checker");

        // Place order
        var orderRequest = new CreateOrderRequest(
            PromoCode: "",
            DeliveryMethod: 1,
            DeliveryCharge: new Price(5.00m, "USD"),
            OrderItems: [new OrderItemRequest(productId, "Black", "L", 1)],
            ShippingAddress: new Address("2 Payment Ave", "Testville", "TV", "Testland", "00002")
        );
        var placeResponse = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/orders/checkout", orderRequest, TestContext.Current.CancellationToken);
        placeResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var order = await placeResponse.Content.ReadFromJsonAsync<OrderDetailResponse>(TestContext.Current.CancellationToken);
        order.ShouldNotBeNull();
        order.OrderStatus.ShouldBe("Pending");
        order.PaymentStatus.ShouldBe("Pending");

        // Act — process payment (MockPaymentService always returns success)
        var paymentRequest = new CardNotPresentSaleRequest(
            OrderId: order.OrderId,
            Amount: order.OrderItems.Sum(i => i.UnitPrice.Amount),
            Currency: order.OrderItems.First().UnitPrice.Currency,
            PaymentMethod: "card",
            PaymentMethodId: "pm_card_visa",
            CardInfo: null
        );
        var paymentResponse = await HttpClient.PostAsJsonAsync(
            $"/api/v1.0/users/{userId}/payments", paymentRequest, TestContext.Current.CancellationToken);
        paymentResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        // Assert — order status should be Processing and payment status Payed
        // PaymentSucceededDomainEvent is handled synchronously by EventualConsistencyMiddleware
        var updatedOrder = await GetOrderAsync(userId, order.OrderId);
        updatedOrder.OrderStatus.ShouldBe("Processing");
        updatedOrder.PaymentStatus.ShouldBe("Payed");
    }
}

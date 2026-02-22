using System.Net;
using System.Net.Http.Json;
using Shopizy.Contracts.Authentication;
using Shopizy.Contracts.Cart;
using Shopizy.Contracts.Category;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Order;
using Shopizy.Contracts.Payment;
using Shopizy.Contracts.Product;
using Shouldly;
using Xunit;

namespace Shopizy.Api.IntegrationTests.Workflows;

public class CustomerPurchaseFlowTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task CustomerPurchaseWorkflow_FullJourney_Succeeds()
    {
        // 1. Setup Phase: Admin creates Category and Product
        var (adminToken, adminUserId) = await AuthenticateAsAdminAsync();
        
        var categoryRequest = new CreateCategoryRequest("Electronics", null);
        var categoryResponse = await HttpClient.PostAsJsonAsync($"/api/v1.0/users/{adminUserId}/categories", categoryRequest, cancellationToken: TestContext.Current.CancellationToken);
        var category = await categoryResponse.Content.ReadFromJsonAsync<CategoryResponse>(cancellationToken: TestContext.Current.CancellationToken);
        
        var productRequest = new CreateProductRequest(
            Name: "Smartphone X",
            ShortDescription: "Latest smartphone",
            Description: "A high-end smartphone with amazing features",
            CategoryId: category!.Id,
            UnitPrice: 999.99m,
            Currency: 1, // USD
            Discount: 0m,
            Sku: "PHONE-X",
            Brand: "GadgetCorp",
            Colors: "Midnight Blue",
            Sizes: "256GB",
            Tags: "phone,electronics",
            Barcode: "987654321",
            SpecificationIds: null
        );
        var productResponse = await HttpClient.PostAsJsonAsync($"/api/v1.0/users/{adminUserId}/products", productRequest, cancellationToken: TestContext.Current.CancellationToken);
        var product = await productResponse.Content.ReadFromJsonAsync<ProductDetailResponse>(cancellationToken: TestContext.Current.CancellationToken);
        
        ClearAuthToken();

        // 2. Customer Phase: Registration and Login
        var customerEmail = $"{Guid.NewGuid().ToString()[..8]}@customer.com";
        var (customerToken, customerUserId) = await AuthenticateAsNewUserAsync("John", "Customer", customerEmail);
        
        // 3. User Journey: Browse Products
        var getProductsResponse = await HttpClient.GetAsync("/api/v1.0/products", TestContext.Current.CancellationToken);
        getProductsResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var products = await getProductsResponse.Content.ReadFromJsonAsync<List<ProductResponse>>(cancellationToken: TestContext.Current.CancellationToken);
        products.ShouldContain(p => p.ProductId == product!.ProductId);

        // 4. User Journey: Add to Cart
        // First get the cart to find its ID
        var getCartResponseInitial = await HttpClient.GetAsync($"/api/v1.0/users/{customerUserId}/carts", TestContext.Current.CancellationToken);
        getCartResponseInitial.StatusCode.ShouldBe(HttpStatusCode.OK);
        var initialCart = await getCartResponseInitial.Content.ReadFromJsonAsync<CartResponse>(TestContext.Current.CancellationToken);
        
        var addToCartRequest = new AddProductToCartRequest(product!.ProductId, "Midnight Blue", "256GB", 1);
        // Cart uses HttpPatch("{cartId:guid}")
        var addToCartResponse = await HttpClient.PatchAsJsonAsync($"/api/v1.0/users/{customerUserId}/carts/{initialCart!.CartId}", addToCartRequest, TestContext.Current.CancellationToken);
        addToCartResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        // 5. User Journey: Review Cart
        var getCartResponse = await HttpClient.GetAsync($"/api/v1.0/users/{customerUserId}/carts", TestContext.Current.CancellationToken);
        getCartResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var cart = await getCartResponse.Content.ReadFromJsonAsync<CartResponse>(TestContext.Current.CancellationToken);
        cart!.CartItems.ShouldContain(i => i.ProductId == product.ProductId);

        // 6. User Journey: Place Order
        var shippingAddress = new Address("123 Main St", "New York", "NY", "USA", "10001");
        var deliveryCharge = new Price(10.00m, "USD");
        var orderItems = new List<OrderItemRequest> { new(product.ProductId, "Midnight Blue", "256GB", 1) };
        var createOrderRequest = new CreateOrderRequest("", 1, deliveryCharge, orderItems, shippingAddress);
        
        var placeOrderResponse = await HttpClient.PostAsJsonAsync($"/api/v1.0/users/{customerUserId}/orders", createOrderRequest, TestContext.Current.CancellationToken);
        if (placeOrderResponse.StatusCode == HttpStatusCode.InternalServerError)
        {
            var errorContent = await placeOrderResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            throw new Exception($"Order placement failed with 500. Response: {errorContent}");
        }
        placeOrderResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var order = await placeOrderResponse.Content.ReadFromJsonAsync<OrderDetailResponse>(TestContext.Current.CancellationToken);
        order.ShouldNotBeNull();

        // 7. User Journey: Process Payment
        var paymentMethodId = "pm_card_visa"; // Mock Stripe payment method ID
        var paymentRequest = new CardNotPresentSaleRequest(
            order.OrderId,
            order.OrderItems[0].UnitPrice.Amount, // Use a representative amount
            order.OrderItems[0].UnitPrice.Currency,
            "card",
            paymentMethodId,
            null
        );
        
        // Payment uses [Route("api/v1.0/users/{userId:guid}/payments")]
        var paymentResponse = await HttpClient.PostAsJsonAsync($"/api/v1.0/users/{customerUserId}/payments", paymentRequest, TestContext.Current.CancellationToken);
        paymentResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var paymentResult = await paymentResponse.Content.ReadFromJsonAsync<SuccessResult>(TestContext.Current.CancellationToken);
        paymentResult!.Message.ShouldNotBeNullOrEmpty();

        // 8. User Journey: Verify Order Status
        var getOrderResponse = await HttpClient.GetAsync($"/api/v1.0/users/{customerUserId}/orders/{order.OrderId}", TestContext.Current.CancellationToken);
        getOrderResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var updatedOrder = await getOrderResponse.Content.ReadFromJsonAsync<OrderDetailResponse>(TestContext.Current.CancellationToken);
        // Status should be Paid or Processing
        updatedOrder!.OrderStatus.ShouldNotBeNullOrEmpty();
        updatedOrder!.OrderStatus.ShouldNotBe("Pending");
    }
}

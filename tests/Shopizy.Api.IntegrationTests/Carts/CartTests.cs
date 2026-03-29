using System.Net;
using Shopizy.Contracts.Cart;
using Shopizy.Contracts.Category;
using Shopizy.Contracts.Product;
using Shouldly;
using Xunit;

namespace Shopizy.Api.IntegrationTests.Carts;

public class CartTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    // --- Setup helpers using correct admin API endpoints ---

    private async Task<Guid> SetupCategoryAsync(string name = "Test Category")
    {
        var request = new CreateCategoryRequest(name, null);
        var response = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/admin/categories", request, TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        var category = await response.Content.ReadFromJsonAsync<CategoryResponse>(TestContext.Current.CancellationToken);
        return category!.Id;
    }

    private async Task<Guid> SetupProductAsync(Guid categoryId, string name = "Test Product")
    {
        var request = new CreateProductRequest(
            name, "Short desc", "Full description", categoryId,
            49.99m, 1, 0m, $"SKU-{Guid.NewGuid().ToString()[..6]}", "TestBrand",
            "Red,Blue", "S,M,L", "test", Guid.NewGuid().ToString()[..8], 100, null);
        var response = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/admin/products", request, TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        var product = await response.Content.ReadFromJsonAsync<ProductResponse>(TestContext.Current.CancellationToken);
        return product!.ProductId;
    }

    // --- Tests ---

    [Fact]
    public async Task GetCart_WhenAuthenticated_ReturnsOk()
    {
        // Arrange
        var (_, userId) = await AuthenticateAsNewUserAsync();

        // Act
        var response = await HttpClient.GetAsync(
            $"/api/v1.0/users/{userId}/cart", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var cart = await response.Content.ReadFromJsonAsync<CartResponse>(TestContext.Current.CancellationToken);
        cart.ShouldNotBeNull();
        cart.UserId.ShouldBe(userId);
    }

    [Fact]
    public async Task GetCart_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthToken();

        // Act
        var response = await HttpClient.GetAsync(
            $"/api/v1.0/users/{Guid.NewGuid()}/cart", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCart_ForAnotherUser_ReturnsForbidden()
    {
        // Arrange
        var (_, user1Id) = await AuthenticateAsNewUserAsync("UserA", "One");
        var (_, user2Id) = await AuthenticateAsNewUserAsync("UserB", "Two");
        // Now authenticated as user2; try to access user1's cart

        // Act
        var response = await HttpClient.GetAsync(
            $"/api/v1.0/users/{user1Id}/cart", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AddProductToCart_WithValidProduct_ReturnsOkWithItem()
    {
        // Arrange
        var (_, adminId) = await AuthenticateAsAdminAsync();
        var categoryId = await SetupCategoryAsync("Cart Electronics");
        var productId = await SetupProductAsync(categoryId, "Cart Headphones");

        var (_, userId) = await AuthenticateAsNewUserAsync("Cart", "Customer");

        var addRequest = new AddProductToCartRequest(productId, "Red", "M", 1);

        // Act
        var response = await HttpClient.PatchAsJsonAsync(
            $"/api/v1.0/users/{userId}/cart/items", addRequest, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var cart = await response.Content.ReadFromJsonAsync<CartResponse>(TestContext.Current.CancellationToken);
        cart.ShouldNotBeNull();
        cart.CartItems.ShouldContain(i => i.ProductId == productId);
    }

    [Fact]
    public async Task AddProductToCart_ForAnotherUser_ReturnsForbidden()
    {
        // Arrange
        var (_, adminId) = await AuthenticateAsAdminAsync();
        var categoryId = await SetupCategoryAsync("Forbidden Cart Category");
        var productId = await SetupProductAsync(categoryId, "Forbidden Cart Product");

        var (_, user1Id) = await AuthenticateAsNewUserAsync("CartA", "User");
        var (_, user2Id) = await AuthenticateAsNewUserAsync("CartB", "User");
        // Now authenticated as user2

        var addRequest = new AddProductToCartRequest(productId, "Blue", "L", 1);

        // Act — attempt to modify user1's cart
        var response = await HttpClient.PatchAsJsonAsync(
            $"/api/v1.0/users/{user1Id}/cart/items", addRequest, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateCartItemQuantity_WithValidItem_ReturnsOk()
    {
        // Arrange
        var (_, adminId) = await AuthenticateAsAdminAsync();
        var categoryId = await SetupCategoryAsync("Qty Category");
        var productId = await SetupProductAsync(categoryId, "Qty Product");

        var (_, userId) = await AuthenticateAsNewUserAsync("Qty", "Customer");

        // Add item first
        var addResponse = await HttpClient.PatchAsJsonAsync(
            $"/api/v1.0/users/{userId}/cart/items",
            new AddProductToCartRequest(productId, "Red", "S", 1),
            TestContext.Current.CancellationToken);
        addResponse.EnsureSuccessStatusCode();
        var cart = await addResponse.Content.ReadFromJsonAsync<CartResponse>(TestContext.Current.CancellationToken);
        var itemId = cart!.CartItems.First(i => i.ProductId == productId).CartItemId;

        // Act — update quantity to 3
        var updateRequest = new UpdateProductQuantityRequest(3);
        var response = await HttpClient.PatchAsJsonAsync(
            $"/api/v1.0/users/{userId}/cart/items/{itemId}", updateRequest, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var updatedCart = await response.Content.ReadFromJsonAsync<CartResponse>(TestContext.Current.CancellationToken);
        updatedCart!.CartItems.First(i => i.CartItemId == itemId).Quantity.ShouldBe(3);
    }

    [Fact]
    public async Task RemoveCartItem_WithValidItem_ReturnsOkWithEmptyCart()
    {
        // Arrange
        var (_, adminId) = await AuthenticateAsAdminAsync();
        var categoryId = await SetupCategoryAsync("Remove Category");
        var productId = await SetupProductAsync(categoryId, "Remove Product");

        var (_, userId) = await AuthenticateAsNewUserAsync("Remove", "Customer");

        // Add item first
        var addResponse = await HttpClient.PatchAsJsonAsync(
            $"/api/v1.0/users/{userId}/cart/items",
            new AddProductToCartRequest(productId, "Blue", "M", 2),
            TestContext.Current.CancellationToken);
        addResponse.EnsureSuccessStatusCode();
        var cart = await addResponse.Content.ReadFromJsonAsync<CartResponse>(TestContext.Current.CancellationToken);
        var itemId = cart!.CartItems.First(i => i.ProductId == productId).CartItemId;

        // Act — remove the item
        var response = await HttpClient.DeleteAsync(
            $"/api/v1.0/users/{userId}/cart/items/{itemId}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var updatedCart = await response.Content.ReadFromJsonAsync<CartResponse>(TestContext.Current.CancellationToken);
        updatedCart!.CartItems.ShouldNotContain(i => i.CartItemId == itemId);
    }

    [Fact]
    public async Task RemoveCartItem_ForAnotherUser_ReturnsForbidden()
    {
        // Arrange
        var (_, user1Id) = await AuthenticateAsNewUserAsync("DelA", "User");
        var (_, user2Id) = await AuthenticateAsNewUserAsync("DelB", "User");
        // Now authenticated as user2; try to delete from user1's cart

        // Act
        var response = await HttpClient.DeleteAsync(
            $"/api/v1.0/users/{user1Id}/cart/items/{Guid.NewGuid()}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}

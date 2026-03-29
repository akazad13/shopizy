using System.Net;
using Shopizy.Contracts.Category;
using Shopizy.Contracts.Product;
using Shopizy.Contracts.Wishlist;
using Shouldly;
using Xunit;

namespace Shopizy.Api.IntegrationTests.Wishlists;

public class WishlistTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    private async Task<Guid> SetupCategoryAsync(string name = "Wishlist Category")
    {
        await AuthenticateAsAdminAsync();
        var request = new CreateCategoryRequest(name, null);
        var response = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/admin/categories", request, TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        var category = await response.Content.ReadFromJsonAsync<CategoryResponse>(TestContext.Current.CancellationToken);
        return category!.Id;
    }

    private async Task<Guid> SetupProductAsync(Guid categoryId, string name = "Wishlist Product")
    {
        var request = new CreateProductRequest(
            name, "Short desc", "Full description", categoryId,
            29.99m, 1, 0m, $"WL-{Guid.NewGuid().ToString()[..6]}", "WishBrand",
            "White", "One Size", "wishlist", Guid.NewGuid().ToString()[..8], 50, null);
        var response = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/admin/products", request, TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        var product = await response.Content.ReadFromJsonAsync<ProductResponse>(TestContext.Current.CancellationToken);
        return product!.ProductId;
    }

    [Fact]
    public async Task CreateWishlist_WhenAuthenticated_ReturnsCreated()
    {
        // Arrange
        var (_, userId) = await AuthenticateAsNewUserAsync("Wish", "Creator");

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            $"/api/v1.0/users/{userId}/wishlist",
            new { },
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var wishlist = await response.Content.ReadFromJsonAsync<WishlistResponse>(TestContext.Current.CancellationToken);
        wishlist.ShouldNotBeNull();
        wishlist.UserId.ShouldBe(userId);
        wishlist.WishlistItems.ShouldBeEmpty();
    }

    [Fact]
    public async Task CreateWishlist_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthToken();

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            $"/api/v1.0/users/{Guid.NewGuid()}/wishlist",
            new { },
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateWishlist_ForAnotherUser_ReturnsForbidden()
    {
        // Arrange
        var (_, user1Id) = await AuthenticateAsNewUserAsync("WishA", "User");
        var (_, user2Id) = await AuthenticateAsNewUserAsync("WishB", "User");
        // Authenticated as user2; attempt to create wishlist for user1

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            $"/api/v1.0/users/{user1Id}/wishlist",
            new { },
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateWishlist_Twice_ReturnsConflict()
    {
        // Arrange
        var (_, userId) = await AuthenticateAsNewUserAsync("WishDup", "User");

        // Create the first wishlist
        var firstResponse = await HttpClient.PostAsJsonAsync(
            $"/api/v1.0/users/{userId}/wishlist", new { }, TestContext.Current.CancellationToken);
        firstResponse.StatusCode.ShouldBe(HttpStatusCode.Created);

        // Act — attempt to create a second wishlist
        var secondResponse = await HttpClient.PostAsJsonAsync(
            $"/api/v1.0/users/{userId}/wishlist", new { }, TestContext.Current.CancellationToken);

        // Assert
        secondResponse.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetWishlist_AfterCreation_ReturnsWishlist()
    {
        // Arrange
        var (_, userId) = await AuthenticateAsNewUserAsync("WishGet", "User");
        await HttpClient.PostAsJsonAsync(
            $"/api/v1.0/users/{userId}/wishlist", new { }, TestContext.Current.CancellationToken);

        // Act
        var response = await HttpClient.GetAsync(
            $"/api/v1.0/users/{userId}/wishlist", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var wishlist = await response.Content.ReadFromJsonAsync<WishlistResponse>(TestContext.Current.CancellationToken);
        wishlist.ShouldNotBeNull();
        wishlist.UserId.ShouldBe(userId);
    }

    [Fact]
    public async Task GetWishlist_ForAnotherUser_ReturnsForbidden()
    {
        // Arrange
        var (_, user1Id) = await AuthenticateAsNewUserAsync("WishGetA", "User");
        await HttpClient.PostAsJsonAsync(
            $"/api/v1.0/users/{user1Id}/wishlist", new { }, TestContext.Current.CancellationToken);

        var (_, user2Id) = await AuthenticateAsNewUserAsync("WishGetB", "User");
        // Authenticated as user2; attempt to read user1's wishlist

        // Act
        var response = await HttpClient.GetAsync(
            $"/api/v1.0/users/{user1Id}/wishlist", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetWishlist_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthToken();

        // Act
        var response = await HttpClient.GetAsync(
            $"/api/v1.0/users/{Guid.NewGuid()}/wishlist", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateWishlist_AddProduct_ReturnsOkWithItem()
    {
        // Arrange
        var categoryId = await SetupCategoryAsync("WL Update Cat");
        var productId = await SetupProductAsync(categoryId, "WL Add Product");

        var (_, userId) = await AuthenticateAsNewUserAsync("WishUpdate", "User");
        await HttpClient.PostAsJsonAsync(
            $"/api/v1.0/users/{userId}/wishlist", new { }, TestContext.Current.CancellationToken);

        var updateRequest = new UpdateWishlistRequest(productId, "Add");

        // Act
        var response = await HttpClient.PatchAsJsonAsync(
            $"/api/v1.0/users/{userId}/wishlist", updateRequest, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var wishlist = await response.Content.ReadFromJsonAsync<WishlistResponse>(TestContext.Current.CancellationToken);
        wishlist.ShouldNotBeNull();
        wishlist.WishlistItems.ShouldContain(i => i.ProductId == productId);
    }

    [Fact]
    public async Task UpdateWishlist_RemoveProduct_ReturnsOkWithoutItem()
    {
        // Arrange
        var categoryId = await SetupCategoryAsync("WL Remove Cat");
        var productId = await SetupProductAsync(categoryId, "WL Remove Product");

        var (_, userId) = await AuthenticateAsNewUserAsync("WishRemove", "User");
        await HttpClient.PostAsJsonAsync(
            $"/api/v1.0/users/{userId}/wishlist", new { }, TestContext.Current.CancellationToken);

        // Add the product first
        await HttpClient.PatchAsJsonAsync(
            $"/api/v1.0/users/{userId}/wishlist",
            new UpdateWishlistRequest(productId, "Add"),
            TestContext.Current.CancellationToken);

        // Act — remove it
        var removeRequest = new UpdateWishlistRequest(productId, "Remove");
        var response = await HttpClient.PatchAsJsonAsync(
            $"/api/v1.0/users/{userId}/wishlist", removeRequest, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var wishlist = await response.Content.ReadFromJsonAsync<WishlistResponse>(TestContext.Current.CancellationToken);
        wishlist.ShouldNotBeNull();
        wishlist.WishlistItems.ShouldNotContain(i => i.ProductId == productId);
    }

    [Fact]
    public async Task UpdateWishlist_ForAnotherUser_ReturnsForbidden()
    {
        // Arrange
        var (_, user1Id) = await AuthenticateAsNewUserAsync("WishModA", "User");
        await HttpClient.PostAsJsonAsync(
            $"/api/v1.0/users/{user1Id}/wishlist", new { }, TestContext.Current.CancellationToken);

        var (_, user2Id) = await AuthenticateAsNewUserAsync("WishModB", "User");
        // Authenticated as user2; attempt to modify user1's wishlist

        // Act
        var response = await HttpClient.PatchAsJsonAsync(
            $"/api/v1.0/users/{user1Id}/wishlist",
            new UpdateWishlistRequest(Guid.NewGuid(), "Add"),
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}

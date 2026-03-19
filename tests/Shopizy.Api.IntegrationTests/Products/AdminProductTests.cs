using System.Net;
using Shopizy.Contracts.Category;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.Product;
using Shouldly;
using Xunit;

namespace Shopizy.Api.IntegrationTests.Products;

public class AdminProductTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    private async Task<Guid> SetupCategoryAsync(string name = "Admin Product Category")
    {
        await AuthenticateAsAdminAsync();
        var request = new CreateCategoryRequest(name, null);
        var response = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/admin/categories", request, TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        var category = await response.Content.ReadFromJsonAsync<CategoryResponse>(TestContext.Current.CancellationToken);
        return category!.Id;
    }

    private CreateProductRequest BuildProductRequest(Guid categoryId, string name = "Admin Test Product") =>
        new(name, "Short description", "Full product description", categoryId,
            199.99m, 1, 10m, $"ADM-{Guid.NewGuid().ToString()[..6]}", "AdminBrand",
            "Black,White", "S,M,L,XL", "admin,test", Guid.NewGuid().ToString()[..8], 50, null);

    [Fact]
    public async Task CreateProduct_AsAdmin_ReturnsOk()
    {
        // Arrange
        var categoryId = await SetupCategoryAsync("Create Product Cat");
        var request = BuildProductRequest(categoryId, "New Admin Product");

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/admin/products", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var product = await response.Content.ReadFromJsonAsync<ProductResponse>(TestContext.Current.CancellationToken);
        product.ShouldNotBeNull();
        product.Name.ShouldBe("New Admin Product");
        product.CategoryId.ShouldBe(categoryId);
        product.StockQuantity.ShouldBe(50);
    }

    [Fact]
    public async Task CreateProduct_AsCustomer_ReturnsForbidden()
    {
        // Arrange
        await AuthenticateAsNewUserAsync("Regular", "Customer");
        var request = BuildProductRequest(Guid.NewGuid(), "Unauthorized Product");

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/admin/products", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateProduct_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthToken();
        var request = BuildProductRequest(Guid.NewGuid(), "Unauth Product");

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/admin/products", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateProduct_AsAdmin_ReturnsOkWithSuccessMessage()
    {
        // Arrange
        var categoryId = await SetupCategoryAsync("Update Product Cat");
        var createRequest = BuildProductRequest(categoryId, "Product Before Update");
        var createResponse = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/admin/products", createRequest, TestContext.Current.CancellationToken);
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<ProductResponse>(TestContext.Current.CancellationToken);

        var updateRequest = new UpdateProductRequest(
            "Product After Update", "Updated short desc", "Updated full description",
            categoryId, 249.99m, 1, 5m, created!.Barcode, "UpdatedBrand",
            "Green", "L,XL", "updated", created.Barcode, null);

        // Act
        var response = await HttpClient.PutAsJsonAsync(
            $"/api/v1.0/admin/products/{created.ProductId}", updateRequest, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<SuccessResult>(TestContext.Current.CancellationToken);
        result.ShouldNotBeNull();
        result.Message.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task UpdateProduct_AsCustomer_ReturnsForbidden()
    {
        // Arrange
        await AuthenticateAsNewUserAsync("Upd", "Customer");
        var updateRequest = new UpdateProductRequest(
            "Hacked", "Hacked desc", "Hacked desc", Guid.NewGuid(),
            1m, 1, 0m, "HACK", "Hack", "Red", "S", "hack", "000", null);

        // Act
        var response = await HttpClient.PutAsJsonAsync(
            $"/api/v1.0/admin/products/{Guid.NewGuid()}", updateRequest, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteProduct_AsAdmin_ReturnsOk()
    {
        // Arrange
        var categoryId = await SetupCategoryAsync("Delete Product Cat");
        var createRequest = BuildProductRequest(categoryId, "Product To Delete");
        var createResponse = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/admin/products", createRequest, TestContext.Current.CancellationToken);
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<ProductResponse>(TestContext.Current.CancellationToken);

        // Act
        var response = await HttpClient.DeleteAsync(
            $"/api/v1.0/admin/products/{created!.ProductId}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        // Verify the product no longer exists
        var getResponse = await HttpClient.GetAsync(
            $"/api/v1.0/products/{created.ProductId}", TestContext.Current.CancellationToken);
        getResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteProduct_AsCustomer_ReturnsForbidden()
    {
        // Arrange
        await AuthenticateAsNewUserAsync("Del", "Customer");

        // Act
        var response = await HttpClient.DeleteAsync(
            $"/api/v1.0/admin/products/{Guid.NewGuid()}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetBrands_Anonymous_ReturnsOkWithList()
    {
        // Arrange — create a product with a distinct brand so the brands list is non-empty
        var categoryId = await SetupCategoryAsync("Brands Category");
        var createRequest = BuildProductRequest(categoryId, "Branded Product");
        await HttpClient.PostAsJsonAsync("/api/v1.0/admin/products", createRequest, TestContext.Current.CancellationToken);
        ClearAuthToken();

        // Act
        var response = await HttpClient.GetAsync(
            "/api/v1.0/brands", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var brands = await response.Content.ReadFromJsonAsync<List<BrandResponse>>(TestContext.Current.CancellationToken);
        brands.ShouldNotBeNull();
        brands.ShouldContain(b => b.Name == "AdminBrand");
    }
}

using System.Net;
using Shopizy.Contracts.Category;
using Shouldly;
using Xunit;

namespace Shopizy.Api.IntegrationTests.Categories;

public class CategoryTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task CreateAndGetCategory_ReturnsCategory()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var categoryName = $"Category_{Guid.NewGuid().ToString()[..8]}";

        // Act
        var categoryId = await CreateCategoryAsync(categoryName);
        var categories = await ListCategoriesAsync();

        // Assert
        categories.ShouldNotBeEmpty();
        categories.Any(c => c.Id == categoryId && c.Name == categoryName).ShouldBeTrue();
    }

    [Fact]
    public async Task DeleteCategory_WhenAdmin_ReturnsNoContent()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var categoryId = await CreateCategoryAsync("DeleteMe");

        // Act
        var response = await HttpClient.DeleteAsync($"/api/v1.0/admin/categories/{categoryId}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        
        var categories = await ListCategoriesAsync();
        categories.Any(c => c.Id == categoryId).ShouldBeFalse();
    }

    [Fact]
    public async Task UpdateCategory_WhenAdmin_ReturnsOk()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var categoryId = await CreateCategoryAsync("OldName");
        var updateRequest = new UpdateCategoryRequest("NewName", null);

        // Act
        var response = await HttpClient.PatchAsJsonAsync($"/api/v1.0/admin/categories/{categoryId}", updateRequest, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCategory_ReturnsCategory()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var categoryId = await CreateCategoryAsync("GetMe");

        // Act
        var response = await HttpClient.GetAsync($"/api/v1.0/categories/{categoryId}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}

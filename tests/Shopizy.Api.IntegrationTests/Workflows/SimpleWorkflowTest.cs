using System.Net;
using Shouldly;
using Xunit;
using Shopizy.Contracts.Product;
using Shopizy.Contracts.Category;
using Shopizy.Contracts.Authentication;
using System.Reflection;
using Shopizy.Domain.Users;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Domain.Permissions.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using Shopizy.Application.Common.Interfaces.Authentication;
using Shopizy.Domain.Carts;

namespace Shopizy.Api.IntegrationTests.Workflows;

/// <summary>
/// Simple end-to-end test demonstrating the correct approach using actual contract types.
/// Use this as a template for fixing other workflow tests.
/// </summary>
public class SimpleWorkflowTest(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task SimpleProductWorkflow_CreateCategoryAndProduct_SuccessfullyCreatesAndRetrieves()
    {
        // 1. Authenticate as Admin
        var (token, userId) = await AuthenticateAsAdminAsync();

        // 2. Create a category
        var categoryRequest = new CreateCategoryRequest("Electronics", null);
        var categoryResponse = await HttpClient.PostAsJsonAsync(
            $"/api/v1.0/users/{userId}/categories",
            categoryRequest,
            TestContext.Current.CancellationToken);
        if (!categoryResponse.IsSuccessStatusCode)
        {
            var errorContent = await categoryResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            throw new InvalidOperationException($"Category creation failed with status {categoryResponse.StatusCode}. Error: {errorContent}");
        }

        var category = await categoryResponse.Content.ReadFromJsonAsync<CategoryResponse>(
            TestContext.Current.CancellationToken);
        category.ShouldNotBeNull();
        category.Name.ShouldBe("Electronics");

        // 4. Create a product
        var productRequest = new CreateProductRequest(
            Name: "Laptop Pro",
            ShortDescription: "High performance laptop",
            Description: "A powerful laptop for professionals",
            CategoryId: category.Id,
            UnitPrice: 1200.00m,
            Currency: 1, // USD
            Discount: 0m,
            Sku: "LAP-001",
            Brand: "TechBrand",
            Colors: "Silver,Black",
            Sizes: "15-inch,17-inch",
            Tags: "laptop,computer",
            Barcode: "123456789",
            SpecificationIds: null
        );

        var productResponse = await HttpClient.PostAsJsonAsync(
            $"/api/v1.0/users/{userId}/products",
            productRequest,
            TestContext.Current.CancellationToken);
        productResponse.EnsureSuccessStatusCode();

        var product = await productResponse.Content.ReadFromJsonAsync<ProductDetailResponse>(
            TestContext.Current.CancellationToken);
        product.ShouldNotBeNull();
        product.Name.ShouldBe("Laptop Pro");
        product.CategoryId.ShouldBe(category.Id);

        // 5. Retrieve the product
        var getProductResponse = await HttpClient.GetAsync(
            $"/api/v1.0/products/{product.ProductId}",
            TestContext.Current.CancellationToken);
        getProductResponse.EnsureSuccessStatusCode();

        var retrievedProduct = await getProductResponse.Content.ReadFromJsonAsync<ProductDetailResponse>(
            TestContext.Current.CancellationToken);
        retrievedProduct.ShouldNotBeNull();
        retrievedProduct.Name.ShouldBe("Laptop Pro");

        // 6. Search for the product
        var searchResponse = await HttpClient.GetAsync(
            "/api/v1.0/products?name=Laptop",
            TestContext.Current.CancellationToken);
        searchResponse.EnsureSuccessStatusCode();

        var products = await searchResponse.Content.ReadFromJsonAsync<List<ProductResponse>>(
            TestContext.Current.CancellationToken);
        products.ShouldNotBeNull();
        products.ShouldContain(p => p.ProductId == product.ProductId);
    }
}

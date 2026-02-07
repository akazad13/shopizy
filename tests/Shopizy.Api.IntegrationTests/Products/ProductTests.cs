using System.Net;
using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Contracts.Product;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Common.Enums;
using Shouldly;
using Xunit;

namespace Shopizy.Api.IntegrationTests.Products;

public class ProductTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetProduct_WithValidId_ReturnsProduct()
    {
        // Arrange
        var category = Shopizy.Domain.Categories.Category.Create("Test Category", null);
        DbContext.Categories.Add(category);

        // Create a product via direct DB context to seed data
        var product = Shopizy.Domain.Products.Product.Create(
            Constants.Product.Name,
            Constants.Product.ShortDescription,
            Constants.Product.Description,
            category.Id,
            Constants.Product.Sku,
            100, // StockQuantity
            Price.CreateNew(100, Currency.usd),
            null,
            Constants.Product.Brand,
            Constants.Product.Barcode,
            Constants.Product.Colors,
            Constants.Product.Sizes,
            Constants.Product.Tags
        );

        DbContext.Products.Add(product);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await HttpClient.GetAsync($"/api/v1.0/products/{product.Id.Value}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var productResponse = await response.Content.ReadFromJsonAsync<ProductDetailResponse>(TestContext.Current.CancellationToken);
        productResponse.ShouldNotBeNull();
        productResponse.ProductId.ShouldBe(product.Id.Value);
        productResponse.Name.ShouldBe(Constants.Product.Name);
    }

    [Fact]
    public async Task SearchProducts_WhenProductsExist_ReturnsList()
    {
        // Arrange - Seed multiple products
        var category = Shopizy.Domain.Categories.Category.Create("Test Category", null);
        DbContext.Categories.Add(category);

         var product1 = Shopizy.Domain.Products.Product.Create(
            "Product A",
            "Short Desc A",
            "Desc A",
            category.Id,
            "SKU-A",
            100, // StockQuantity
            Price.CreateNew(50, Currency.usd),
            null,
            "BrandA",
            "BarcodeA",
            "Red",
            "L",
            "TagA"
        );
         var product2 = Shopizy.Domain.Products.Product.Create(
            "Product B",
            "Short Desc B",
            "Desc B",
            category.Id,
            "SKU-B",
            100, // StockQuantity
            Price.CreateNew(150, Currency.usd),
            null,
            "BrandB",
            "BarcodeB",
            "Blue",
            "M",
            "TagB"
        );

        DbContext.Products.AddRange(product1, product2);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await HttpClient.GetAsync("/api/v1.0/products", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var products = await response.Content.ReadFromJsonAsync<List<ProductResponse>>();
        products.ShouldNotBeNull();
        products.Count.ShouldBeGreaterThanOrEqualTo(2);
        products.ShouldContain(p => p.ProductId == product1.Id.Value);
        products.ShouldContain(p => p.ProductId == product2.Id.Value);
    }
}


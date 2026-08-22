using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Domain.Brands;
using Shopizy.Domain.Categories;
using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Products;
using Shopizy.Infrastructure.Common.Persistence;
using Shopizy.Infrastructure.Services.Search;
using Shouldly;
using Xunit;

namespace Shopizy.Infrastructure.UnitTests.Services;

public class ProductSearchEngineTests
{
    private readonly Mock<ILogger<ProductSearchEngine>> _mockLogger = new();
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor = new();

    private AppDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options, _mockHttpContextAccessor.Object);
    }

    [Fact]
    public async Task SearchProductsAsync_SynonymResolution_ShouldFindProduct()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var category = Category.Create("Footwear", null);
        var product = Product.Create(
            name: "Classic Running Shoes",
            shortDescription: "Running shoes",
            description: "High performance athletic shoes",
            categoryId: category.Id,
            sku: "SKU-SHOE-1",
            stockQuantity: 25,
            unitPrice: Price.CreateNew(80m, Currency.usd),
            discount: null,
            brandId: null,
            barcode: "12345678",
            colors: "Black,White",
            sizes: "9,10,11",
            tags: "athletic,cushioned"
        );

        dbContext.Categories.Add(category);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        var sut = new ProductSearchEngine(dbContext, _mockLogger.Object);
        var query = new ProductSearchQueryDto(SearchTerm: "sneakers");

        // Act
        var result = await sut.SearchProductsAsync(query);

        // Assert
        result.TotalCount.ShouldBe(1);
        result.Items[0].Name.ShouldBe("Classic Running Shoes");
    }

    [Fact]
    public async Task SearchProductsAsync_FuzzyTypoMatch_ShouldFindProductAndSuggestKeywords()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var category = Category.Create("Electronics", null);
        var product = Product.Create(
            name: "Apple iPhone 15 Pro",
            shortDescription: "Flagship phone",
            description: "Titanium mobile phone with A17 Pro",
            categoryId: category.Id,
            sku: "SKU-IPHONE-15",
            stockQuantity: 10,
            unitPrice: Price.CreateNew(999m, Currency.usd),
            discount: null,
            brandId: null,
            barcode: "87654321",
            colors: "Natural Titanium,Black",
            sizes: "128GB,256GB",
            tags: "apple,5g"
        );

        dbContext.Categories.Add(category);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        var sut = new ProductSearchEngine(dbContext, _mockLogger.Object);
        var query = new ProductSearchQueryDto(SearchTerm: "iphne");

        // Act
        var result = await sut.SearchProductsAsync(query);

        // Assert
        result.TotalCount.ShouldBe(1);
        result.Items[0].Name.ShouldBe("Apple iPhone 15 Pro");
        result.SuggestedKeywords.ShouldContain("iphone");
    }

    [Fact]
    public async Task SearchProductsAsync_FacetAggregation_ShouldComputeAccurateFacets()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var cat1 = Category.Create("Clothing", null);
        var cat2 = Category.Create("Footwear", null);

        var p1 = Product.Create(
            "Cotton T-Shirt",
            "Tee",
            "Soft tee",
            cat1.Id,
            "SKU-TSHIRT",
            15,
            Price.CreateNew(20m, Currency.usd),
            null,
            null,
            "1111",
            "White",
            "M,L",
            "casual"
        );
        var p2 = Product.Create(
            "Denim Jacket",
            "Jacket",
            "Winter jacket",
            cat1.Id,
            "SKU-JACKET",
            5,
            Price.CreateNew(120m, Currency.usd),
            null,
            null,
            "2222",
            "Blue",
            "L,XL",
            "winter"
        );
        var p3 = Product.Create(
            "Leather Boots",
            "Boots",
            "Sturdy boots",
            cat2.Id,
            "SKU-BOOTS",
            8,
            Price.CreateNew(220m, Currency.usd),
            null,
            null,
            "3333",
            "Brown",
            "10,11",
            "leather"
        );

        dbContext.Categories.AddRange(cat1, cat2);
        dbContext.Products.AddRange(p1, p2, p3);
        await dbContext.SaveChangesAsync();

        var sut = new ProductSearchEngine(dbContext, _mockLogger.Object);
        var query = new ProductSearchQueryDto(SearchTerm: null);

        // Act
        var result = await sut.SearchProductsAsync(query);

        // Assert
        result.TotalCount.ShouldBe(3);
        var categoryFacet = result.Facets.FirstOrDefault(f => f.FieldName == "Category");
        categoryFacet.ShouldNotBeNull();
        categoryFacet.Values.Count.ShouldBe(2);

        var priceFacet = result.Facets.FirstOrDefault(f => f.FieldName == "Price");
        priceFacet.ShouldNotBeNull();
        priceFacet.Values.Any(v => v.Key == "under_25" && v.Count == 1).ShouldBeTrue();
        priceFacet.Values.Any(v => v.Key == "200_plus" && v.Count == 1).ShouldBeTrue();
    }
}

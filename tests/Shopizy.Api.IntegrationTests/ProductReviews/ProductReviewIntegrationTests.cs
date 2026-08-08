using System.Net;
using System.Net.Http.Json;
using Shopizy.Contracts.ProductReview;
using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Common.ValueObjects;
using Shouldly;
using Xunit;

namespace Shopizy.Api.IntegrationTests.ProductReviews;

public class ProductReviewIntegrationTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetProductReviews_WhenNoReviews_ShouldReturnEmptyList()
    {
        // Arrange - Seed product directly via DbContext
        var category = Shopizy.Domain.Categories.Category.Create("Review Test Cat", null);
        DbContext.Categories.Add(category);

        var product = Shopizy.Domain.Products.Product.Create(
            "Review Test Prod",
            "Short",
            "Long",
            category.Id,
            $"REV-SKU-{Guid.NewGuid().ToString()[..6]}",
            10,
            Price.CreateNew(100, Currency.usd),
            null,
            null,
            Guid.NewGuid().ToString()[..8],
            "Red",
            "M",
            "test"
        );
        DbContext.Products.Add(product);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var response = await HttpClient.GetAsync(
            $"api/v1.0/products/{product.Id.Value}/reviews?pageNumber=1&pageSize=10",
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var reviews = await response.Content.ReadFromJsonAsync<List<ProductReviewResponse>>(
            TestContext.Current.CancellationToken
        );
        reviews.ShouldNotBeNull();
    }

    [Fact]
    public async Task CreateProductReview_WhenAuthenticatedUser_ShouldReturnReview()
    {
        // Arrange - Seed category and product
        var category = Shopizy.Domain.Categories.Category.Create("Review Create Cat", null);
        DbContext.Categories.Add(category);

        var product = Shopizy.Domain.Products.Product.Create(
            "Review Create Prod",
            "Short",
            "Long",
            category.Id,
            $"REV-CR-{Guid.NewGuid().ToString()[..6]}",
            15,
            Price.CreateNew(60, Currency.usd),
            null,
            null,
            Guid.NewGuid().ToString()[..8],
            "Blue",
            "L",
            "review"
        );
        DbContext.Products.Add(product);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Authenticate admin user (has all permissions including ProductReview.Create)
        await AuthenticateAsAdminAsync();

        var createReq = new CreateProductReviewRequest(Rating: 5m, Comment: "Excellent product!");

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            $"api/v1.0/products/{product.Id.Value}/reviews",
            createReq,
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var review = await response.Content.ReadFromJsonAsync<ProductReviewResponse>(
            TestContext.Current.CancellationToken
        );
        review.ShouldNotBeNull();
        review.Rating.ShouldBe(5m);
        review.Comment.ShouldBe("Excellent product!");
    }
}

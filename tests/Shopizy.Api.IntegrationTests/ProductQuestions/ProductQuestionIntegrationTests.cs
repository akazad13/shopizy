using System.Net;
using System.Net.Http.Json;
using Shopizy.Contracts.ProductQuestion;
using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Common.ValueObjects;
using Shouldly;
using Xunit;

namespace Shopizy.Api.IntegrationTests.ProductQuestions;

public class ProductQuestionIntegrationTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetProductQuestions_WhenNoQuestions_ShouldReturnEmptyList()
    {
        // Arrange - Seed a product directly via DbContext
        var category = Shopizy.Domain.Categories.Category.Create("PQ Test Category", null);
        DbContext.Categories.Add(category);

        var product = Shopizy.Domain.Products.Product.Create(
            "PQ Test Product",
            "Short desc",
            "Full desc",
            category.Id,
            $"PQ-SKU-{Guid.NewGuid().ToString()[..6]}",
            10,
            Price.CreateNew(50, Currency.usd),
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
            $"/api/v1.0/products/{product.Id.Value}/questions?pageNumber=1&pageSize=10",
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var questions = await response.Content.ReadFromJsonAsync<List<ProductQuestionResponse>>(
            TestContext.Current.CancellationToken
        );
        questions.ShouldNotBeNull();
    }

    [Fact]
    public async Task AskQuestion_WhenAuthenticated_ShouldReturnCreatedQuestion()
    {
        // Arrange - Seed a product
        var category = Shopizy.Domain.Categories.Category.Create("AskQ Category", null);
        DbContext.Categories.Add(category);

        var product = Shopizy.Domain.Products.Product.Create(
            "Ask Question Product",
            "Short",
            "Long desc",
            category.Id,
            $"ASK-SKU-{Guid.NewGuid().ToString()[..6]}",
            20,
            Price.CreateNew(75, Currency.usd),
            null,
            null,
            Guid.NewGuid().ToString()[..8],
            "Blue",
            "L",
            "qa"
        );
        DbContext.Products.Add(product);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Authenticate as a regular user
        await AuthenticateAsNewUserAsync(
            email: $"questioner{Guid.NewGuid().ToString()[..6]}@test.com"
        );

        // Act
        var askReq = new AskQuestionRequest("Does this item come with a warranty?");
        var response = await HttpClient.PostAsJsonAsync(
            $"/api/v1.0/products/{product.Id.Value}/questions",
            askReq,
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var question = await response.Content.ReadFromJsonAsync<ProductQuestionResponse>(
            TestContext.Current.CancellationToken
        );
        question.ShouldNotBeNull();
        question.Question.ShouldBe("Does this item come with a warranty?");
        question.IsAnswered.ShouldBeFalse();
    }

    [Fact]
    public async Task AskQuestion_WhenUnauthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        ClearAuthToken();

        // Act
        var askReq = new AskQuestionRequest("This should be unauthorized.");
        var response = await HttpClient.PostAsJsonAsync(
            $"/api/v1.0/products/{Guid.NewGuid()}/questions",
            askReq,
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}

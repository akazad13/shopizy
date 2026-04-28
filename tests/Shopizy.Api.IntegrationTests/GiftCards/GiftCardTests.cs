using System.Net;
using Shopizy.Contracts.GiftCard;
using Shouldly;
using Xunit;

namespace Shopizy.Api.IntegrationTests.GiftCards;

public class GiftCardTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    private static CreateGiftCardRequest CreateRequest(string? code = null) =>
        new(code ?? $"GIFT-{Guid.NewGuid().ToString()[..8].ToUpper()}", 50.00m, null);

    [Fact]
    public async Task CreateGiftCard_AsAdmin_ReturnsOkWithGiftCard()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var request = CreateRequest();

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/admin/gift-cards",
            request,
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var giftCard = await response.Content.ReadFromJsonAsync<GiftCardResponse>(
            TestContext.Current.CancellationToken
        );
        giftCard.ShouldNotBeNull();
        giftCard.Code.ShouldBe(request.Code);
        giftCard.InitialBalance.ShouldBe(request.InitialBalance);
        giftCard.IsActive.ShouldBeTrue();
    }

    [Fact]
    public async Task CreateGiftCard_AsCustomer_ReturnsForbidden()
    {
        // Arrange
        await AuthenticateAsNewUserAsync("GC", "Customer");

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/admin/gift-cards",
            CreateRequest(),
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateGiftCard_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthToken();

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/admin/gift-cards",
            CreateRequest(),
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateGiftCard_DuplicateCode_ReturnsConflict()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var request = CreateRequest();

        // Create the first gift card
        var firstResponse = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/admin/gift-cards",
            request,
            TestContext.Current.CancellationToken
        );
        firstResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        // Act — attempt to create a second gift card with the same code
        var secondResponse = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/admin/gift-cards",
            request,
            TestContext.Current.CancellationToken
        );

        // Assert
        secondResponse.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetGiftCards_AsAdmin_ReturnsOkWithList()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        await HttpClient.PostAsJsonAsync(
            "/api/v1.0/admin/gift-cards",
            CreateRequest(),
            TestContext.Current.CancellationToken
        );

        // Act
        var response = await HttpClient.GetAsync(
            "/api/v1.0/admin/gift-cards?pageNumber=1&pageSize=10",
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var giftCards = await response.Content.ReadFromJsonAsync<IReadOnlyList<GiftCardResponse>>(
            TestContext.Current.CancellationToken
        );
        giftCards.ShouldNotBeNull();
        giftCards.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task GetGiftCards_AsCustomer_ReturnsForbidden()
    {
        // Arrange
        await AuthenticateAsNewUserAsync("GCList", "Customer");

        // Act
        var response = await HttpClient.GetAsync(
            "/api/v1.0/admin/gift-cards?pageNumber=1&pageSize=10",
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ValidateGiftCard_WithValidCode_ReturnsOkWithGiftCard()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var createRequest = CreateRequest();
        var createResponse = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/admin/gift-cards",
            createRequest,
            TestContext.Current.CancellationToken
        );
        createResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        // Authenticate as customer to validate
        await AuthenticateAsNewUserAsync("GCValidate", "Customer");

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/gift-cards/validate",
            createRequest.Code,
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var giftCard = await response.Content.ReadFromJsonAsync<GiftCardResponse>(
            TestContext.Current.CancellationToken
        );
        giftCard.ShouldNotBeNull();
        giftCard.Code.ShouldBe(createRequest.Code);
        giftCard.IsActive.ShouldBeTrue();
    }

    [Fact]
    public async Task ValidateGiftCard_WithInvalidCode_ReturnsNotFound()
    {
        // Arrange
        await AuthenticateAsNewUserAsync("GCInvalid", "Customer");

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/gift-cards/validate",
            "NONEXISTENT-CODE",
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ValidateGiftCard_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthToken();

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            "/api/v1.0/gift-cards/validate",
            "ANY-CODE",
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}

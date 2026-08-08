using System.Net;
using System.Net.Http.Json;
using Shopizy.Contracts.PromoCode;
using Shouldly;
using Xunit;

namespace Shopizy.Api.IntegrationTests.PromoCodes;

public class PromoCodeIntegrationTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task CreatePromoCode_AsAdmin_ShouldReturnOk()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        var createReq = new CreatePromoCodeRequest(
            Code: $"SUMMER{Guid.NewGuid().ToString()[..4].ToUpper()}",
            Description: "Summer sale discount",
            Discount: 15m,
            IsPercentage: true,
            IsActive: true
        );

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            "api/v1.0/admin/promo-codes",
            createReq,
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var promo = await response.Content.ReadFromJsonAsync<PromoCodeResponse>(
            TestContext.Current.CancellationToken
        );
        promo.ShouldNotBeNull();
        promo.Code.ShouldBe(createReq.Code);
        promo.Discount.ShouldBe(15m);
        promo.IsActive.ShouldBeTrue();
    }

    [Fact]
    public async Task GetPromoCodes_AsAdmin_ShouldReturnList()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        // Seed at least one promo code
        var createReq = new CreatePromoCodeRequest(
            Code: $"LIST{Guid.NewGuid().ToString()[..4].ToUpper()}",
            Description: "Test promo for listing",
            Discount: 10m,
            IsPercentage: false,
            IsActive: true
        );
        await HttpClient.PostAsJsonAsync(
            "api/v1.0/admin/promo-codes",
            createReq,
            TestContext.Current.CancellationToken
        );

        // Act
        var response = await HttpClient.GetAsync(
            "api/v1.0/admin/promo-codes?pageNumber=1&pageSize=20",
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<PromoCodeResponse>>(
            TestContext.Current.CancellationToken
        );
        list.ShouldNotBeNull();
        list.ShouldContain(p => p.Code == createReq.Code);
    }

    [Fact]
    public async Task ValidatePromoCode_WhenActive_ShouldReturnPromoCodeDetails()
    {
        // Arrange - Create promo code as admin
        await AuthenticateAsAdminAsync();
        var code = $"WELCOME{Guid.NewGuid().ToString()[..4].ToUpper()}";
        var createReq = new CreatePromoCodeRequest(code, "Welcome discount", 10m, false, true);
        await HttpClient.PostAsJsonAsync(
            "api/v1.0/admin/promo-codes",
            createReq,
            TestContext.Current.CancellationToken
        );

        // Authenticate as a regular user
        var (_, userId) = await AuthenticateAsNewUserAsync(
            email: $"promovalidate{Guid.NewGuid().ToString()[..6]}@test.com"
        );

        // Act
        var validateResp = await HttpClient.PostAsJsonAsync(
            $"api/v1.0/users/{userId}/orders/validate-promo",
            code,
            TestContext.Current.CancellationToken
        );

        // Assert
        validateResp.StatusCode.ShouldBe(HttpStatusCode.OK);
        var promo = await validateResp.Content.ReadFromJsonAsync<PromoCodeResponse>(
            TestContext.Current.CancellationToken
        );
        promo.ShouldNotBeNull();
        promo.Code.ShouldBe(code);
    }

    [Fact]
    public async Task CreatePromoCode_AsRegularUser_ShouldReturnForbidden()
    {
        // Arrange
        await AuthenticateAsNewUserAsync(
            email: $"nopromo{Guid.NewGuid().ToString()[..6]}@test.com"
        );

        var createReq = new CreatePromoCodeRequest("FORBIDDEN", "Should fail", 5m, false, true);

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            "api/v1.0/admin/promo-codes",
            createReq,
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}

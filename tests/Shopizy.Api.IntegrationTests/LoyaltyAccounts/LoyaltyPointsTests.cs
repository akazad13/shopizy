using System.Net;
using Shopizy.Contracts.LoyaltyAccount;
using Shouldly;
using Xunit;

namespace Shopizy.Api.IntegrationTests.LoyaltyAccounts;

public class LoyaltyPointsTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task EarnPoints_AsAdmin_ReturnsOkWithUpdatedBalance()
    {
        // Arrange
        var (_, userId) = await AuthenticateAsNewUserAsync("Loyal", "Earner");
        await AuthenticateAsAdminAsync();
        var request = new EarnPointsRequest(100, "Welcome bonus");

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            $"/api/v1.0/users/{userId}/loyalty/earn", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var account = await response.Content.ReadFromJsonAsync<LoyaltyAccountResponse>(TestContext.Current.CancellationToken);
        account.ShouldNotBeNull();
        account.TotalPoints.ShouldBe(100);
    }

    [Fact]
    public async Task EarnPoints_AsCustomer_ReturnsForbidden()
    {
        // Arrange
        var (_, userId) = await AuthenticateAsNewUserAsync("Loyal", "Forbidden");
        var request = new EarnPointsRequest(100, "Test");

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            $"/api/v1.0/users/{userId}/loyalty/earn", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task EarnPoints_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthToken();
        var request = new EarnPointsRequest(100, "Test");

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            $"/api/v1.0/users/{Guid.NewGuid()}/loyalty/earn", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RedeemPoints_AfterEarning_ReturnsOkWithReducedBalance()
    {
        // Arrange — register user, then earn points as admin
        var (userToken, userId) = await AuthenticateAsNewUserAsync("Loyal", "Redeemer");
        await AuthenticateAsAdminAsync();
        await HttpClient.PostAsJsonAsync(
            $"/api/v1.0/users/{userId}/loyalty/earn",
            new EarnPointsRequest(200, "Initial credit"),
            TestContext.Current.CancellationToken);

        // Switch back to user token to redeem
        SetAuthToken(userToken);
        var redeemRequest = new RedeemPointsRequest(50, "Discount redemption");

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            $"/api/v1.0/users/{userId}/loyalty/redeem", redeemRequest, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var account = await response.Content.ReadFromJsonAsync<LoyaltyAccountResponse>(TestContext.Current.CancellationToken);
        account.ShouldNotBeNull();
        account.TotalPoints.ShouldBe(150);
    }

    [Fact]
    public async Task RedeemPoints_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthToken();
        var request = new RedeemPointsRequest(50, "Test");

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            $"/api/v1.0/users/{Guid.NewGuid()}/loyalty/redeem", request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RedeemPoints_ForAnotherUser_ReturnsForbidden()
    {
        // Arrange
        var (_, user1Id) = await AuthenticateAsNewUserAsync("LoyalA", "User");
        await AuthenticateAsNewUserAsync("LoyalB", "User");
        // Now authenticated as user2; attempt to redeem for user1

        // Act
        var response = await HttpClient.PostAsJsonAsync(
            $"/api/v1.0/users/{user1Id}/loyalty/redeem",
            new RedeemPointsRequest(10, "Test"),
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetLoyaltyAccount_AfterEarning_ReturnsOkWithCorrectBalance()
    {
        // Arrange
        var (userToken, userId) = await AuthenticateAsNewUserAsync("Loyal", "Getter");
        await AuthenticateAsAdminAsync();
        await HttpClient.PostAsJsonAsync(
            $"/api/v1.0/users/{userId}/loyalty/earn",
            new EarnPointsRequest(75, "Test credit"),
            TestContext.Current.CancellationToken);

        SetAuthToken(userToken);

        // Act
        var response = await HttpClient.GetAsync(
            $"/api/v1.0/users/{userId}/loyalty", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var account = await response.Content.ReadFromJsonAsync<LoyaltyAccountResponse>(TestContext.Current.CancellationToken);
        account.ShouldNotBeNull();
        account.TotalPoints.ShouldBe(75);
        account.Transactions.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task GetLoyaltyAccount_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthToken();

        // Act
        var response = await HttpClient.GetAsync(
            $"/api/v1.0/users/{Guid.NewGuid()}/loyalty", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}

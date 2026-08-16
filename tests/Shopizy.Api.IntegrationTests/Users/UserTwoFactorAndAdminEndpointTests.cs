using System.Net;
using System.Net.Http.Json;
using Shopizy.Contracts.Admin;
using Shopizy.Contracts.Authentication;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.User;
using Shouldly;
using Xunit;

namespace Shopizy.Api.IntegrationTests.Users;

public class UserTwoFactorAndAdminEndpointTests : BaseIntegrationTest
{
    public UserTwoFactorAndAdminEndpointTests(IntegrationTestWebAppFactory factory)
        : base(factory) { }

    [Fact]
    public async Task TwoFactorAuthentication_EnableAndDisable_Succeeds()
    {
        // 1. Authenticate user
        var (_, userId) = await AuthenticateAsNewUserAsync(
            "TwoFactorUser",
            "Test",
            "2fa@example.com"
        );

        // 2. Enable 2FA
        var enableResponse = await HttpClient.PostAsync(
            $"/api/v1.0/users/{userId}/two-factor/enable",
            null,
            TestContext.Current.CancellationToken
        );

        enableResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var setupResult = await enableResponse.Content.ReadFromJsonAsync<TwoFactorSetupResponse>(
            TestContext.Current.CancellationToken
        );
        setupResult.ShouldNotBeNull();
        setupResult.Secret.ShouldNotBeNullOrEmpty();

        // 3. Disable 2FA
        var disableResponse = await HttpClient.DeleteAsync(
            $"/api/v1.0/users/{userId}/two-factor",
            TestContext.Current.CancellationToken
        );

        disableResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Admin_GetUsers_WhenAdmin_ReturnsPagedResponse()
    {
        // 1. Authenticate as Admin
        await AuthenticateAsAdminAsync();

        // 2. Act
        var response = await HttpClient.GetAsync(
            "/api/v1.0/admin/users?pageNumber=1&pageSize=10",
            TestContext.Current.CancellationToken
        );

        // 3. Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponse<UserDetails>>(
            TestContext.Current.CancellationToken
        );
        pagedResponse.ShouldNotBeNull();
        pagedResponse.Items.ShouldNotBeNull();
    }

    [Fact]
    public async Task Admin_UpdateUserRole_WhenAdmin_ReturnsOk()
    {
        // 1. Create a regular user
        var (_, regularUserId) = await AuthenticateAsNewUserAsync(
            "TargetUser",
            "Test",
            "targetrole@example.com"
        );

        // 2. Authenticate as Admin
        await AuthenticateAsAdminAsync();

        // 3. Act
        var updateRoleRequest = new UpdateUserRoleRequest("Admin", new List<Guid>());
        var response = await HttpClient.PatchAsJsonAsync(
            $"/api/v1.0/admin/users/{regularUserId}/role",
            updateRoleRequest,
            TestContext.Current.CancellationToken
        );

        // 4. Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}

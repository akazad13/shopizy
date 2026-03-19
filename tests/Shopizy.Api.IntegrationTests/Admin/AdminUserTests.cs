using System.Net;
using Microsoft.EntityFrameworkCore;
using Shopizy.Contracts.Admin;
using Shopizy.Contracts.Common;
using Shopizy.Contracts.User;
using Shouldly;
using Xunit;

namespace Shopizy.Api.IntegrationTests.Admin;

public class AdminUserTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllUsers_AsAdmin_ReturnsOkWithList()
    {
        // Arrange — ensure at least one customer user exists
        await AuthenticateAsNewUserAsync("Listed", "User");
        await AuthenticateAsAdminAsync();

        // Act
        var response = await HttpClient.GetAsync(
            "/api/v1.0/admin/users?pageNumber=1&pageSize=10", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var users = await response.Content.ReadFromJsonAsync<List<UserDetails>>(TestContext.Current.CancellationToken);
        users.ShouldNotBeNull();
        users.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task GetAllUsers_AsCustomer_ReturnsForbidden()
    {
        // Arrange
        await AuthenticateAsNewUserAsync("Forbid", "User");

        // Act
        var response = await HttpClient.GetAsync(
            "/api/v1.0/admin/users?pageNumber=1&pageSize=10", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAllUsers_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthToken();

        // Act
        var response = await HttpClient.GetAsync(
            "/api/v1.0/admin/users?pageNumber=1&pageSize=10", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAllUsers_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        // Act
        var response = await HttpClient.GetAsync(
            "/api/v1.0/admin/users?pageNumber=1&pageSize=5", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var users = await response.Content.ReadFromJsonAsync<List<UserDetails>>(TestContext.Current.CancellationToken);
        users.ShouldNotBeNull();
        users.Count.ShouldBeLessThanOrEqualTo(5);
    }

    [Fact]
    public async Task UpdateUserRole_AsAdmin_ReturnsOkWithSuccessMessage()
    {
        // Arrange
        var (_, customerId) = await AuthenticateAsNewUserAsync("RoleChange", "User");
        await AuthenticateAsAdminAsync();

        // Fetch all permission IDs so we can assign customer permissions
        var allPermissions = await DbContext.Permissions.ToListAsync(TestContext.Current.CancellationToken);
        var customerPermissionIds = allPermissions
            .Where(p => p.Name.StartsWith("User.") || p.Name.StartsWith("Cart.") ||
                        p.Name.StartsWith("Order.") || p.Name.StartsWith("Wishlist."))
            .Select(p => p.Id.Value)
            .ToList();

        var updateRoleRequest = new UpdateUserRoleRequest("Customer", customerPermissionIds);

        // Act
        var response = await HttpClient.PatchAsJsonAsync(
            $"/api/v1.0/admin/users/{customerId}/role", updateRoleRequest, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<SuccessResult>(TestContext.Current.CancellationToken);
        result.ShouldNotBeNull();
        result.Message.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task UpdateUserRole_PromoteToAdmin_ReturnsOk()
    {
        // Arrange
        var (_, customerId) = await AuthenticateAsNewUserAsync("ToAdmin", "User");
        await AuthenticateAsAdminAsync();

        var allPermissions = await DbContext.Permissions.ToListAsync(TestContext.Current.CancellationToken);
        var allPermissionIds = allPermissions.Select(p => p.Id.Value).ToList();

        var updateRoleRequest = new UpdateUserRoleRequest("Admin", allPermissionIds);

        // Act
        var response = await HttpClient.PatchAsJsonAsync(
            $"/api/v1.0/admin/users/{customerId}/role", updateRoleRequest, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateUserRole_AsCustomer_ReturnsForbidden()
    {
        // Arrange
        var (_, targetUserId) = await AuthenticateAsNewUserAsync("RoleTarget", "User");
        await AuthenticateAsNewUserAsync("RoleAttacker", "User");
        // Authenticated as another customer; try to update roles

        // Act
        var response = await HttpClient.PatchAsJsonAsync(
            $"/api/v1.0/admin/users/{targetUserId}/role",
            new UpdateUserRoleRequest("Admin", []),
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateUserRole_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthToken();

        // Act
        var response = await HttpClient.PatchAsJsonAsync(
            $"/api/v1.0/admin/users/{Guid.NewGuid()}/role",
            new UpdateUserRoleRequest("Admin", []),
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}

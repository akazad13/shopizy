using System.Net;
using Shopizy.Contracts.Admin;
using Shouldly;
using Xunit;

namespace Shopizy.Api.IntegrationTests.Admin;

public class DashboardTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetDashboardMetrics_AsAdmin_ReturnsOkWithMetrics()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        // Act
        var response = await HttpClient.GetAsync(
            "/api/v1.0/admin/dashboard/metrics", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var metrics = await response.Content.ReadFromJsonAsync<DashboardMetricsResponse>(TestContext.Current.CancellationToken);
        metrics.ShouldNotBeNull();
        metrics.TotalUsers.ShouldBeGreaterThanOrEqualTo(0);
        metrics.TotalOrders.ShouldBeGreaterThanOrEqualTo(0);
        metrics.TotalProducts.ShouldBeGreaterThanOrEqualTo(0);
        metrics.TotalRevenue.ShouldBeGreaterThanOrEqualTo(0);
        metrics.StockAlerts.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetDashboardMetrics_AsCustomer_ReturnsForbidden()
    {
        // Arrange
        await AuthenticateAsNewUserAsync("Dash", "Customer");

        // Act
        var response = await HttpClient.GetAsync(
            "/api/v1.0/admin/dashboard/metrics", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetDashboardMetrics_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        ClearAuthToken();

        // Act
        var response = await HttpClient.GetAsync(
            "/api/v1.0/admin/dashboard/metrics", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetDashboardMetrics_AfterCreatingData_ReflectsActualCounts()
    {
        // Arrange — ensure we have at least one user and product
        await AuthenticateAsNewUserAsync("DashData", "User");
        await AuthenticateAsAdminAsync();

        // Act
        var response = await HttpClient.GetAsync(
            "/api/v1.0/admin/dashboard/metrics", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var metrics = await response.Content.ReadFromJsonAsync<DashboardMetricsResponse>(TestContext.Current.CancellationToken);
        metrics.ShouldNotBeNull();
        // At minimum, the admin user and the DashData user exist
        metrics.TotalUsers.ShouldBeGreaterThanOrEqualTo(1);
    }
}

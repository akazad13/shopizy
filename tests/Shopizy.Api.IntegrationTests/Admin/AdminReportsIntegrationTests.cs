using System.Net;
using System.Net.Http.Json;
using Shopizy.Contracts.Admin;
using Shouldly;
using Xunit;

namespace Shopizy.Api.IntegrationTests.Admin;

public class AdminReportsIntegrationTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetSalesReport_AsAdmin_ShouldReturnSalesReport()
    {
        // Arrange
        await AuthenticateAsAdminAsync();
        var start = DateTime.UtcNow.AddDays(-7).ToString("yyyy-MM-ddTHH:mm:ssZ");
        var end = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

        // Act
        var response = await HttpClient.GetAsync(
            $"api/v1.0/admin/reports/sales?startDate={start}&endDate={end}",
            TestContext.Current.CancellationToken
        );

        // Assert
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new Exception($"GetSalesReport Status: {response.StatusCode}, Body: {body}");
        }

        var report = await response.Content.ReadFromJsonAsync<SalesReportResponse>(
            TestContext.Current.CancellationToken
        );
        report.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetTopCustomers_AsAdmin_ShouldReturnTopCustomersList()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        // Act
        var response = await HttpClient.GetAsync(
            "api/v1.0/admin/reports/customers/top?count=5",
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var customers = await response.Content.ReadFromJsonAsync<List<TopCustomerResponse>>(
            TestContext.Current.CancellationToken
        );
        customers.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetTopProducts_AsAdmin_ShouldReturnTopProductsList()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        // Act
        var response = await HttpClient.GetAsync(
            "api/v1.0/admin/reports/products/top?count=5",
            TestContext.Current.CancellationToken
        );

        // Assert
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new Exception($"GetTopProducts Status: {response.StatusCode}, Body: {body}");
        }

        var products = await response.Content.ReadFromJsonAsync<List<TopProductResponse>>(
            TestContext.Current.CancellationToken
        );
        products.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetReports_AsRegularUser_ShouldReturnForbidden()
    {
        // Arrange
        await AuthenticateAsNewUserAsync(email: $"unauth{Guid.NewGuid().ToString()[..6]}@test.com");

        // Act
        var response = await HttpClient.GetAsync(
            "api/v1.0/admin/reports/customers/top?count=5",
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}

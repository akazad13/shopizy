using System.Net;
using System.Net.Http.Json;
using Shopizy.Contracts.AuditLog;
using Shouldly;
using Xunit;

namespace Shopizy.Api.IntegrationTests.AuditLogs;

public class AuditLogsIntegrationTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAuditLogs_AsAdmin_ShouldReturnAuditLogs()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        // Act
        var response = await HttpClient.GetAsync(
            "api/v1.0/admin/audit-logs?pageNumber=1&pageSize=10",
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var logs = await response.Content.ReadFromJsonAsync<List<AuditLogResponse>>(
            TestContext.Current.CancellationToken
        );
        logs.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetAuditLogs_AsRegularUser_ShouldReturnForbidden()
    {
        // Arrange
        await AuthenticateAsNewUserAsync(
            email: $"noaudit{Guid.NewGuid().ToString()[..6]}@test.com"
        );

        // Act
        var response = await HttpClient.GetAsync(
            "api/v1.0/admin/audit-logs?pageNumber=1&pageSize=10",
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}

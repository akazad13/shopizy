using System.Net;
using System.Text;
using Shouldly;
using Xunit;

namespace Shopizy.Api.IntegrationTests.Payments;

public class StripeWebhookEndpointTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    private const string WebhookEndpoint = "/api/v1.0/webhooks/stripe";

    [Fact]
    public async Task StripeWebhook_WithoutSignatureHeader_ShouldReturnBadRequest()
    {
        // Arrange
        var content = new StringContent("{}", Encoding.UTF8, "application/json");

        // Act
        var response = await HttpClient.PostAsync(
            WebhookEndpoint,
            content,
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        body.ShouldContain("stripe.missing_signature");
    }

    [Fact]
    public async Task StripeWebhook_WithInvalidSignatureHeader_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, WebhookEndpoint)
        {
            Content = new StringContent("{\"id\":\"evt_test\"}", Encoding.UTF8, "application/json"),
        };
        request.Headers.Add("Stripe-Signature", "t=123456,v1=invalidsignature");

        // Act
        var response = await HttpClient.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        body.ShouldContain("Invalid signature");
    }
}

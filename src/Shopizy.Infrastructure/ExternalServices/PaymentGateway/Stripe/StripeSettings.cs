using System.Diagnostics.CodeAnalysis;

namespace Shopizy.Infrastructure.ExternalServices.PaymentGateway.Stripe;

[ExcludeFromCodeCoverage]
public class StripeSettings
{
    public const string Section = "StripeSettings";
    public string SecretKey { get; set; } = string.Empty;
    public string PublishableKey { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
}

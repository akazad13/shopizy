namespace Shopizy.Infrastructure.ExternalServices.PaymentGateway.Stripe;

public class StripeSettings
{
    public const string Section = "StripeSettings";
    public string SecretKey { get; set; } = string.Empty;
    public string PublishableKey { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
}

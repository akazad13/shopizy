namespace Shopizy.Infrastructure.ExternalServices.PaymentGateway.Stripe;

public class StripeSettings
{
    public const string Section = "StripeSettings";
    public string SecretKey { get; set; } = string.Empty;
    public string publishableKey { get; set; } = string.Empty;
}

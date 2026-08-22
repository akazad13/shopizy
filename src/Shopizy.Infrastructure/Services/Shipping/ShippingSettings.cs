namespace Shopizy.Infrastructure.Services.Shipping;

/// <summary>
/// Settings for shipping carrier integrations and rate estimation defaults.
/// </summary>
public class ShippingSettings
{
    public const string Section = "ShippingSettings";

    /// <summary>
    /// Free shipping subtotal qualification threshold (e.g. $100).
    /// </summary>
    public decimal FreeShippingThreshold { get; set; } = 100m;

    /// <summary>
    /// Base standard shipping fee (e.g. $5.99).
    /// </summary>
    public decimal BaseStandardRate { get; set; } = 5.99m;

    /// <summary>
    /// Base express shipping fee (e.g. $14.99).
    /// </summary>
    public decimal BaseExpressRate { get; set; } = 14.99m;

    /// <summary>
    /// Base overnight shipping fee (e.g. $29.99).
    /// </summary>
    public decimal BaseOvernightRate { get; set; } = 29.99m;

    /// <summary>
    /// Additional cost per kilogram of package weight.
    /// </summary>
    public decimal WeightRatePerKg { get; set; } = 1.50m;
}

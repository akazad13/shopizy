using System.Diagnostics.CodeAnalysis;

namespace Shopizy.Infrastructure.Carts;

/// <summary>
/// Configuration options for cart recovery and management.
/// </summary>
[ExcludeFromCodeCoverage]
public class CartSettings
{
    public const string Section = "CartSettings";

    /// <summary>
    /// Hours of user inactivity required before a cart is classified as abandoned.
    /// </summary>
    public int AbandonedCartInactivityHours { get; set; } = 2;

    /// <summary>
    /// Background check frequency interval in minutes.
    /// </summary>
    public int CheckIntervalMinutes { get; set; } = 30;
}

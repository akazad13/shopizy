using System.Diagnostics.CodeAnalysis;

namespace Shopizy.Infrastructure.Orders;

/// <summary>
/// Configuration options for order processing and expiration.
/// </summary>
[ExcludeFromCodeCoverage]
public class OrderSettings
{
    public const string Section = "OrderSettings";

    /// <summary>
    /// The number of minutes a pending order is held before automatically expiring.
    /// </summary>
    public int PendingOrderExpirationMinutes { get; set; } = 15;

    /// <summary>
    /// The interval in seconds between background worker checks for expired orders.
    /// </summary>
    public int ExpirationCheckIntervalSeconds { get; set; } = 60;
}

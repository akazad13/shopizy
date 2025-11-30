namespace Shopizy.Contracts.Order;

/// <summary>
/// Represents a request to cancel an order.
/// </summary>
/// <param name="Reason">The reason for cancellation.</param>
public record CancelOrderRequest(string Reason);

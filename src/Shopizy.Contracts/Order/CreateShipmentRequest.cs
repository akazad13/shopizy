namespace Shopizy.Contracts.Order;

/// <summary>
/// Creates a shipment for an order. Equivalent to <see cref="AddShipmentRequest"/> but used by
/// the dedicated POST endpoint.
/// </summary>
/// <param name="Carrier">Shipping carrier (e.g., UPS, FedEx).</param>
/// <param name="TrackingNumber">Carrier-issued tracking number.</param>
/// <param name="EstimatedDelivery">Optional estimated delivery date.</param>
public record CreateShipmentRequest(
    string Carrier,
    string TrackingNumber,
    DateTime? EstimatedDelivery
);

namespace Shopizy.Contracts.Order;

/// <summary>
/// Attaches a shipment to an existing order.
/// </summary>
/// <param name="Carrier">Shipping carrier (e.g., UPS, FedEx).</param>
/// <param name="TrackingNumber">Carrier-issued tracking number.</param>
/// <param name="EstimatedDelivery">Optional estimated delivery date.</param>
public record AddShipmentRequest(
    string Carrier,
    string TrackingNumber,
    DateTime? EstimatedDelivery
);

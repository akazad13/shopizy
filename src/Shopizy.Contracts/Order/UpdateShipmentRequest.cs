namespace Shopizy.Contracts.Order;

/// <summary>
/// Updates the carrier, tracking, and status of an existing shipment.
/// </summary>
/// <param name="Carrier">Shipping carrier.</param>
/// <param name="TrackingNumber">Carrier-issued tracking number.</param>
/// <param name="EstimatedDelivery">Carrier's estimated delivery date.</param>
/// <param name="Status">Target status as the underlying ShipmentStatus integer value.</param>
public record UpdateShipmentRequest(
    string Carrier,
    string TrackingNumber,
    DateTime? EstimatedDelivery,
    int Status
);

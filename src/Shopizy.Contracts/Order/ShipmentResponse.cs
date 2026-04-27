namespace Shopizy.Contracts.Order;

/// <summary>
/// Shipment information attached to an order.
/// </summary>
/// <param name="ShipmentId">Identifier of the shipment.</param>
/// <param name="Carrier">Shipping carrier.</param>
/// <param name="TrackingNumber">Carrier-issued tracking number.</param>
/// <param name="EstimatedDelivery">Carrier's estimated delivery date, when known.</param>
/// <param name="Status">Shipment status (string form of the underlying enum).</param>
/// <param name="CreatedOn">UTC timestamp when the shipment record was created.</param>
public record ShipmentResponse(
    Guid ShipmentId,
    string Carrier,
    string TrackingNumber,
    DateTime? EstimatedDelivery,
    string Status,
    DateTime CreatedOn
);

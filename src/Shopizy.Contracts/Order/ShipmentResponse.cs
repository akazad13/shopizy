namespace Shopizy.Contracts.Order;

public record ShipmentResponse(
    Guid ShipmentId,
    string Carrier,
    string TrackingNumber,
    DateTime? EstimatedDelivery,
    string Status,
    DateTime CreatedOn
);

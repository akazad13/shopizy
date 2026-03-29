namespace Shopizy.Contracts.Order;

public record CreateShipmentRequest(
    string Carrier,
    string TrackingNumber,
    DateTime? EstimatedDelivery
);

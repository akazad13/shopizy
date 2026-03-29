namespace Shopizy.Contracts.Order;

public record UpdateShipmentRequest(
    string Carrier,
    string TrackingNumber,
    DateTime? EstimatedDelivery,
    int Status
);

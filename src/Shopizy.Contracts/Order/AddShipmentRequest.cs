namespace Shopizy.Contracts.Order;

public record AddShipmentRequest(string Carrier, string TrackingNumber, DateTime? EstimatedDelivery);

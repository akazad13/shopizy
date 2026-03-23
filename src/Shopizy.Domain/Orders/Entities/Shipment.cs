using Shopizy.Domain.Orders.Enums;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Orders.Entities;

public sealed class Shipment : Entity<ShipmentId>
{
    public string Carrier { get; private set; }
    public string TrackingNumber { get; private set; }
    public DateTime? EstimatedDelivery { get; private set; }
    public ShipmentStatus Status { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public DateTime? ModifiedOn { get; private set; }

    public static Shipment Create(string carrier, string trackingNumber, DateTime? estimatedDelivery)
        => new Shipment(ShipmentId.CreateUnique(), carrier, trackingNumber, estimatedDelivery);

    public void Update(string carrier, string trackingNumber, DateTime? estimatedDelivery, ShipmentStatus status)
    {
        Carrier = carrier;
        TrackingNumber = trackingNumber;
        EstimatedDelivery = estimatedDelivery;
        Status = status;
    }

    private Shipment(
        ShipmentId id,
        string carrier,
        string trackingNumber,
        DateTime? estimatedDelivery
    )
        : base(id)
    {
        Carrier = carrier;
        TrackingNumber = trackingNumber;
        EstimatedDelivery = estimatedDelivery;
        Status = ShipmentStatus.Pending;
        CreatedOn = DateTime.UtcNow;
    }

    private Shipment() { }
}

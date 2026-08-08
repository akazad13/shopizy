using Shopizy.Domain.Orders.Entities;
using Shopizy.Domain.Orders.Enums;
using Shopizy.Domain.Orders.ValueObjects;
using Shouldly;
using Xunit;

namespace Shopizy.Domain.UnitTests.Orders;

public class ShipmentTests
{
    [Fact]
    public void CreateAndUpdate_ShouldInitializeAndUpdateShipment()
    {
        var estDate = DateTime.UtcNow.AddDays(5);
        var shipment = Shipment.Create("FedEx", "TRACK123", estDate);

        shipment.ShouldNotBeNull();
        shipment.Carrier.ShouldBe("FedEx");
        shipment.TrackingNumber.ShouldBe("TRACK123");
        shipment.EstimatedDelivery.ShouldBe(estDate);
        shipment.Status.ShouldBe(ShipmentStatus.Pending);

        var newEstDate = DateTime.UtcNow.AddDays(3);
        shipment.Update("DHL", "TRACK456", newEstDate, ShipmentStatus.InTransit);

        shipment.Carrier.ShouldBe("DHL");
        shipment.TrackingNumber.ShouldBe("TRACK456");
        shipment.EstimatedDelivery.ShouldBe(newEstDate);
        shipment.Status.ShouldBe(ShipmentStatus.InTransit);
    }

    [Fact]
    public void ShipmentId_CreateUniqueAndCreate_ShouldInitialize()
    {
        var sId1 = ShipmentId.CreateUnique();
        var raw = Guid.NewGuid();
        var sId2 = ShipmentId.Create(raw);

        sId1.Value.ShouldNotBe(Guid.Empty);
        sId2.Value.ShouldBe(raw);
    }
}

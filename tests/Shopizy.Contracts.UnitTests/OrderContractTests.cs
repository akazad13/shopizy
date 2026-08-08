using Shopizy.Contracts.Order;
using Shouldly;
using Xunit;

namespace Shopizy.Contracts.UnitTests;

// ─────────────────────────────────────────────────────────
// Shared helpers
// ─────────────────────────────────────────────────────────
file static class OrderFactory
{
    public static Price MakePrice(decimal amount = 9.99m, string currency = "USD") =>
        new(amount, currency);

    public static Address MakeAddress(string street = "1 Test St") =>
        new(street, "Testville", "TS", "Testland", "T1 1TT");

    public static OrderItemRequest MakeItemRequest(Guid? productId = null) =>
        new(productId ?? Guid.NewGuid(), "Red", "M", 2);

    public static OrderItemResponse MakeItemResponse(Guid? id = null) =>
        new(
            id ?? Guid.NewGuid(),
            "Widget",
            MakePrice(),
            "https://img.test/w.jpg",
            "Blue",
            "L",
            1,
            0m
        );
}

// ─────────────────────────────────────────────────────────
// Price
// ─────────────────────────────────────────────────────────
public class PriceTests
{
    [Fact]
    public void Create_ShouldHoldCorrectValues()
    {
        var sut = new Price(29.99m, "GBP");
        sut.Amount.ShouldBe(29.99m);
        sut.Currency.ShouldBe("GBP");
    }

    [Fact]
    public void TwoInstances_WithSameValues_ShouldBeEqual()
    {
        var a = new Price(10m, "USD");
        var b = new Price(10m, "USD");
        a.ShouldBe(b);
        a.GetHashCode().ShouldBe(b.GetHashCode());
    }

    [Fact]
    public void TwoInstances_WithDifferentCurrency_ShouldNotBeEqual()
    {
        var a = new Price(10m, "USD");
        var b = new Price(10m, "EUR");
        a.ShouldNotBe(b);
    }

    [Fact]
    public void WithExpression_ShouldProduceUpdatedCopy()
    {
        var original = new Price(5m, "USD");
        var updated = original with { Amount = 15m };
        original.Amount.ShouldBe(5m);
        updated.Amount.ShouldBe(15m);
        updated.Currency.ShouldBe("USD");
    }

    [Fact]
    public void Amount_WhenZero_ShouldBeAllowed()
    {
        var sut = new Price(0m, "USD");
        sut.Amount.ShouldBe(0m);
    }
}

// ─────────────────────────────────────────────────────────
// Address
// ─────────────────────────────────────────────────────────
public class AddressTests
{
    [Fact]
    public void Create_ShouldHoldCorrectValues()
    {
        var sut = new Address("10 Main St", "London", "England", "UK", "EC1A 1BB");
        sut.Street.ShouldBe("10 Main St");
        sut.City.ShouldBe("London");
        sut.State.ShouldBe("England");
        sut.Country.ShouldBe("UK");
        sut.ZipCode.ShouldBe("EC1A 1BB");
    }

    [Fact]
    public void TwoInstances_WithSameValues_ShouldBeEqual()
    {
        var a = new Address("S", "C", "ST", "CO", "ZIP");
        var b = new Address("S", "C", "ST", "CO", "ZIP");
        a.ShouldBe(b);
        a.GetHashCode().ShouldBe(b.GetHashCode());
    }

    [Fact]
    public void TwoInstances_WithDifferentZip_ShouldNotBeEqual()
    {
        var a = new Address("S", "C", "ST", "CO", "11111");
        var b = new Address("S", "C", "ST", "CO", "99999");
        a.ShouldNotBe(b);
    }

    [Fact]
    public void WithExpression_ShouldProduceUpdatedCopy()
    {
        var original = new Address("Old", "OC", "OS", "OX", "00000");
        var updated = original with { City = "New City", ZipCode = "11111" };
        original.City.ShouldBe("OC");
        updated.City.ShouldBe("New City");
        updated.ZipCode.ShouldBe("11111");
        updated.Street.ShouldBe(original.Street);
    }
}

// ─────────────────────────────────────────────────────────
// OrderItemRequest
// ─────────────────────────────────────────────────────────
public class OrderItemRequestTests
{
    [Fact]
    public void Create_ShouldHoldCorrectValues()
    {
        var pid = Guid.NewGuid();
        var sut = new OrderItemRequest(pid, "Green", "XL", 3);
        sut.ProductId.ShouldBe(pid);
        sut.Color.ShouldBe("Green");
        sut.Size.ShouldBe("XL");
        sut.Quantity.ShouldBe(3);
    }

    [Fact]
    public void TwoInstances_WithSameValues_ShouldBeEqual()
    {
        var pid = Guid.NewGuid();
        var a = new OrderItemRequest(pid, "Red", "S", 1);
        var b = new OrderItemRequest(pid, "Red", "S", 1);
        a.ShouldBe(b);
        a.GetHashCode().ShouldBe(b.GetHashCode());
    }

    [Fact]
    public void TwoInstances_WithDifferentQuantity_ShouldNotBeEqual()
    {
        var pid = Guid.NewGuid();
        var a = new OrderItemRequest(pid, "Red", "S", 1);
        var b = new OrderItemRequest(pid, "Red", "S", 5);
        a.ShouldNotBe(b);
    }

    [Fact]
    public void WithExpression_ShouldProduceUpdatedCopy()
    {
        var original = new OrderItemRequest(Guid.NewGuid(), "Blue", "M", 2);
        var updated = original with { Color = "Yellow", Quantity = 10 };
        original.Color.ShouldBe("Blue");
        updated.Color.ShouldBe("Yellow");
        updated.Quantity.ShouldBe(10);
        updated.ProductId.ShouldBe(original.ProductId);
    }
}

// ─────────────────────────────────────────────────────────
// CreateOrderRequest
// ─────────────────────────────────────────────────────────
public class CreateOrderRequestTests
{
    [Fact]
    public void Create_ShouldHoldCorrectValues()
    {
        IList<OrderItemRequest> items = [OrderFactory.MakeItemRequest()];
        var sut = new CreateOrderRequest(
            "PROMO10",
            1,
            OrderFactory.MakePrice(),
            items,
            OrderFactory.MakeAddress()
        );

        sut.PromoCode.ShouldBe("PROMO10");
        sut.DeliveryMethod.ShouldBe(1);
        sut.DeliveryCharge.Currency.ShouldBe("USD");
        sut.OrderItems.ShouldNotBeNull();
        sut.OrderItems.Count.ShouldBe(1);
        sut.ShippingAddress.ShouldNotBeNull();
    }

    [Fact]
    public void Create_WithEmptyPromoCode_ShouldBeAllowed()
    {
        var sut = new CreateOrderRequest(
            "",
            1,
            OrderFactory.MakePrice(),
            [],
            OrderFactory.MakeAddress()
        );
        sut.PromoCode.ShouldBe("");
    }

    [Fact]
    public void Create_WithMultipleItems_ShouldPreserveAll()
    {
        IList<OrderItemRequest> items =
        [
            OrderFactory.MakeItemRequest(),
            OrderFactory.MakeItemRequest(),
            OrderFactory.MakeItemRequest(),
        ];
        var sut = new CreateOrderRequest(
            "",
            2,
            OrderFactory.MakePrice(),
            items,
            OrderFactory.MakeAddress()
        );
        sut.OrderItems.Count.ShouldBe(3);
    }

    [Fact]
    public void TwoInstances_WithSameSharedList_ShouldBeEqual()
    {
        IList<OrderItemRequest> sharedItems = [OrderFactory.MakeItemRequest()];
        var addr = OrderFactory.MakeAddress();
        var price = OrderFactory.MakePrice();
        var a = new CreateOrderRequest("X", 1, price, sharedItems, addr);
        var b = new CreateOrderRequest("X", 1, price, sharedItems, addr);
        a.ShouldBe(b);
    }

    [Fact]
    public void WithExpression_ShouldProduceUpdatedCopy()
    {
        var original = new CreateOrderRequest(
            "OLD",
            1,
            OrderFactory.MakePrice(),
            [],
            OrderFactory.MakeAddress()
        );
        var updated = original with { PromoCode = "NEW", DeliveryMethod = 2 };
        original.PromoCode.ShouldBe("OLD");
        updated.PromoCode.ShouldBe("NEW");
        updated.DeliveryMethod.ShouldBe(2);
        updated.ShippingAddress.ShouldBe(original.ShippingAddress);
    }
}

// ─────────────────────────────────────────────────────────
// CancelOrderRequest
// ─────────────────────────────────────────────────────────
public class CancelOrderRequestTests
{
    [Fact]
    public void Create_ShouldHoldReason()
    {
        var sut = new CancelOrderRequest("Changed my mind");
        sut.Reason.ShouldBe("Changed my mind");
    }

    [Fact]
    public void TwoInstances_WithSameReason_ShouldBeEqual()
    {
        var a = new CancelOrderRequest("Out of stock");
        var b = new CancelOrderRequest("Out of stock");
        a.ShouldBe(b);
        a.GetHashCode().ShouldBe(b.GetHashCode());
    }

    [Fact]
    public void TwoInstances_WithDifferentReason_ShouldNotBeEqual()
    {
        var a = new CancelOrderRequest("Reason A");
        var b = new CancelOrderRequest("Reason B");
        a.ShouldNotBe(b);
    }

    [Fact]
    public void WithExpression_ShouldProduceUpdatedCopy()
    {
        var original = new CancelOrderRequest("Old reason");
        var updated = original with { Reason = "New reason" };
        original.Reason.ShouldBe("Old reason");
        updated.Reason.ShouldBe("New reason");
    }
}

// ─────────────────────────────────────────────────────────
// BulkUpdateOrderStatusRequest
// ─────────────────────────────────────────────────────────
public class BulkUpdateOrderStatusRequestTests
{
    [Fact]
    public void Create_ShouldHoldCorrectValues()
    {
        IList<Guid> ids = [Guid.NewGuid(), Guid.NewGuid()];
        var sut = new BulkUpdateOrderStatusRequest(ids, (int)OrderStatus.Shipped);
        sut.Status.ShouldBe(3);
        sut.OrderIds.Count.ShouldBe(2);
    }

    [Fact]
    public void Create_WithEmptyIds_ShouldBeAllowed()
    {
        var sut = new BulkUpdateOrderStatusRequest([], (int)OrderStatus.Pending);
        sut.OrderIds.Count.ShouldBe(0);
        sut.Status.ShouldBe(1);
    }

    [Fact]
    public void TwoInstances_WithSharedListAndSameStatus_ShouldBeEqual()
    {
        IList<Guid> shared = [Guid.NewGuid()];
        var a = new BulkUpdateOrderStatusRequest(shared, 2);
        var b = new BulkUpdateOrderStatusRequest(shared, 2);
        a.ShouldBe(b);
    }

    [Fact]
    public void TwoInstances_WithDifferentStatus_ShouldNotBeEqual()
    {
        IList<Guid> shared = [];
        var a = new BulkUpdateOrderStatusRequest(shared, 1);
        var b = new BulkUpdateOrderStatusRequest(shared, 5);
        a.ShouldNotBe(b);
    }

    [Fact]
    public void WithExpression_ShouldProduceUpdatedCopy()
    {
        IList<Guid> ids = [Guid.NewGuid()];
        var original = new BulkUpdateOrderStatusRequest(ids, (int)OrderStatus.Pending);
        var updated = original with { Status = (int)OrderStatus.Cancelled };
        original.Status.ShouldBe(1);
        updated.Status.ShouldBe(5);
        updated.OrderIds.ShouldBeSameAs(original.OrderIds);
    }
}

// ─────────────────────────────────────────────────────────
// OrderStatus enum
// ─────────────────────────────────────────────────────────
public class OrderStatusTests
{
    [Theory]
    [InlineData(OrderStatus.Pending, 1)]
    [InlineData(OrderStatus.Processing, 2)]
    [InlineData(OrderStatus.Shipped, 3)]
    [InlineData(OrderStatus.Delivered, 4)]
    [InlineData(OrderStatus.Cancelled, 5)]
    [InlineData(OrderStatus.Refunded, 6)]
    public void OrderStatus_ShouldHaveCorrectIntegerValue(OrderStatus status, int expected)
    {
        ((int)status).ShouldBe(expected);
    }

    [Fact]
    public void OrderStatus_ShouldHaveSixValues()
    {
        Enum.GetValues<OrderStatus>().Length.ShouldBe(6);
    }
}

// ─────────────────────────────────────────────────────────
// OrdersCriteria
// ─────────────────────────────────────────────────────────
public class OrdersCriteriaTests
{
    [Fact]
    public void Create_WithDefaults_ShouldUseSensibleDefaults()
    {
        var sut = new OrdersCriteria();
        sut.StartDate.ShouldBeNull();
        sut.EndDate.ShouldBeNull();
        sut.PageNumber.ShouldBe(1);
        sut.PageSize.ShouldBe(10);
        sut.Status.ShouldBeNull();
    }

    [Fact]
    public void Create_WithAllValues_ShouldHoldCorrectValues()
    {
        var start = DateTime.UtcNow.AddDays(-30);
        var end = DateTime.UtcNow;
        var sut = new OrdersCriteria(start, end, 2, 25, OrderStatus.Shipped);

        sut.StartDate.ShouldBe(start);
        sut.EndDate.ShouldBe(end);
        sut.PageNumber.ShouldBe(2);
        sut.PageSize.ShouldBe(25);
        sut.Status.ShouldBe(OrderStatus.Shipped);
    }

    [Fact]
    public void TwoInstances_WithSameValues_ShouldBeEqual()
    {
        var a = new OrdersCriteria(PageNumber: 1, PageSize: 10);
        var b = new OrdersCriteria(PageNumber: 1, PageSize: 10);
        a.ShouldBe(b);
        a.GetHashCode().ShouldBe(b.GetHashCode());
    }

    [Fact]
    public void TwoInstances_WithDifferentPageSize_ShouldNotBeEqual()
    {
        var a = new OrdersCriteria(PageNumber: 1, PageSize: 10);
        var b = new OrdersCriteria(PageNumber: 1, PageSize: 20);
        a.ShouldNotBe(b);
    }

    [Fact]
    public void WithExpression_ShouldProduceUpdatedCopy()
    {
        var original = new OrdersCriteria(Status: OrderStatus.Pending);
        var updated = original with { Status = OrderStatus.Delivered, PageSize = 50 };
        original.Status.ShouldBe(OrderStatus.Pending);
        updated.Status.ShouldBe(OrderStatus.Delivered);
        updated.PageSize.ShouldBe(50);
        updated.PageNumber.ShouldBe(original.PageNumber);
    }
}

// ─────────────────────────────────────────────────────────
// OrderResponse
// ─────────────────────────────────────────────────────────
public class OrderResponseTests
{
    [Fact]
    public void Create_ShouldHoldCorrectValues()
    {
        var oid = Guid.NewGuid();
        var uid = Guid.NewGuid();
        var ts = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc);

        var sut = new OrderResponse(oid, uid, OrderFactory.MakePrice(120m), "Pending", ts);

        sut.OrderId.ShouldBe(oid);
        sut.UserId.ShouldBe(uid);
        sut.Total.Amount.ShouldBe(120m);
        sut.OrderStatus.ShouldBe("Pending");
        sut.CreatedOn.ShouldBe(ts);
    }

    [Fact]
    public void TwoInstances_WithSameValues_ShouldBeEqual()
    {
        var oid = Guid.NewGuid();
        var uid = Guid.NewGuid();
        var ts = DateTime.UtcNow;
        var price = OrderFactory.MakePrice();
        var a = new OrderResponse(oid, uid, price, "Pending", ts);
        var b = new OrderResponse(oid, uid, price, "Pending", ts);
        a.ShouldBe(b);
        a.GetHashCode().ShouldBe(b.GetHashCode());
    }

    [Fact]
    public void TwoInstances_WithDifferentStatus_ShouldNotBeEqual()
    {
        var oid = Guid.NewGuid();
        var uid = Guid.NewGuid();
        var ts = DateTime.UtcNow;
        var price = OrderFactory.MakePrice();
        var a = new OrderResponse(oid, uid, price, "Pending", ts);
        var b = new OrderResponse(oid, uid, price, "Shipped", ts);
        a.ShouldNotBe(b);
    }

    [Fact]
    public void WithExpression_ShouldProduceUpdatedCopy()
    {
        var original = new OrderResponse(
            Guid.NewGuid(),
            Guid.NewGuid(),
            OrderFactory.MakePrice(),
            "Pending",
            DateTime.UtcNow
        );
        var updated = original with { OrderStatus = "Delivered" };
        original.OrderStatus.ShouldBe("Pending");
        updated.OrderStatus.ShouldBe("Delivered");
        updated.OrderId.ShouldBe(original.OrderId);
    }
}

// ─────────────────────────────────────────────────────────
// OrderItemResponse
// ─────────────────────────────────────────────────────────
public class OrderItemResponseTests
{
    [Fact]
    public void Create_ShouldHoldCorrectValues()
    {
        var iid = Guid.NewGuid();
        var sut = new OrderItemResponse(
            iid,
            "Widget",
            OrderFactory.MakePrice(50m),
            "https://img.test/w.jpg",
            "Red",
            "M",
            2,
            5m
        );

        sut.OrderItemId.ShouldBe(iid);
        sut.Name.ShouldBe("Widget");
        sut.UnitPrice.Amount.ShouldBe(50m);
        sut.PictureUrl.ShouldBe("https://img.test/w.jpg");
        sut.Color.ShouldBe("Red");
        sut.Size.ShouldBe("M");
        sut.Quantity.ShouldBe(2);
        sut.Discount.ShouldBe(5m);
    }

    [Fact]
    public void TwoInstances_WithSameValues_ShouldBeEqual()
    {
        var iid = Guid.NewGuid();
        var price = OrderFactory.MakePrice();
        var a = new OrderItemResponse(iid, "Item", price, "url", "Blue", "L", 1, 0m);
        var b = new OrderItemResponse(iid, "Item", price, "url", "Blue", "L", 1, 0m);
        a.ShouldBe(b);
        a.GetHashCode().ShouldBe(b.GetHashCode());
    }

    [Fact]
    public void TwoInstances_WithDifferentDiscount_ShouldNotBeEqual()
    {
        var iid = Guid.NewGuid();
        var price = OrderFactory.MakePrice();
        var a = new OrderItemResponse(iid, "Item", price, "url", "Blue", "L", 1, 0m);
        var b = new OrderItemResponse(iid, "Item", price, "url", "Blue", "L", 1, 10m);
        a.ShouldNotBe(b);
    }

    [Fact]
    public void Discount_WhenZero_ShouldBeAllowed()
    {
        var sut = OrderFactory.MakeItemResponse();
        sut.Discount.ShouldBe(0m);
    }

    [Fact]
    public void WithExpression_ShouldProduceUpdatedCopy()
    {
        var original = OrderFactory.MakeItemResponse();
        var updated = original with { Quantity = 5, Discount = 2.5m };
        original.Quantity.ShouldBe(1);
        updated.Quantity.ShouldBe(5);
        updated.Discount.ShouldBe(2.5m);
        updated.OrderItemId.ShouldBe(original.OrderItemId);
    }
}

// ─────────────────────────────────────────────────────────
// OrderDetailResponse
// ─────────────────────────────────────────────────────────
public class OrderDetailResponseTests
{
    private static OrderDetailResponse MakeSut(
        Guid? orderId = null,
        IList<OrderItemResponse>? items = null
    )
    {
        var ts = DateTime.UtcNow;
        return new OrderDetailResponse(
            orderId ?? Guid.NewGuid(),
            Guid.NewGuid(),
            OrderFactory.MakePrice(5m),
            "Processing",
            "PROMO10",
            OrderFactory.MakeAddress(),
            "Paid",
            items ?? [],
            ts,
            ts
        );
    }

    [Fact]
    public void Create_ShouldHoldCorrectValues()
    {
        IList<OrderItemResponse> items = [OrderFactory.MakeItemResponse()];
        var sut = MakeSut(items: items);

        sut.OrderStatus.ShouldBe("Processing");
        sut.PromoCode.ShouldBe("PROMO10");
        sut.PaymentStatus.ShouldBe("Paid");
        sut.ShippingAddress.ShouldNotBeNull();
        sut.OrderItems.Count.ShouldBe(1);
        sut.DeliveryCharge.Amount.ShouldBe(5m);
    }

    [Fact]
    public void Create_WithEmptyItems_ShouldBeAllowed()
    {
        var sut = MakeSut(items: []);
        sut.OrderItems.Count.ShouldBe(0);
    }

    [Fact]
    public void TwoInstances_WithSameSharedList_ShouldBeEqual()
    {
        var oid = Guid.NewGuid();
        IList<OrderItemResponse> sharedItems = [];
        var a = MakeSut(orderId: oid, items: sharedItems);
        // Re-create with same OrderId, same shared list reference
        var ts = a.CreatedOn;
        var b = new OrderDetailResponse(
            oid,
            a.UserId,
            a.DeliveryCharge,
            a.OrderStatus,
            a.PromoCode,
            a.ShippingAddress,
            a.PaymentStatus,
            sharedItems,
            ts,
            ts
        );
        a.ShouldBe(b);
    }

    [Fact]
    public void TwoInstances_WithDifferentStatus_ShouldNotBeEqual()
    {
        var oid = Guid.NewGuid();
        var a = MakeSut(orderId: oid);
        var b = new OrderDetailResponse(
            oid,
            a.UserId,
            a.DeliveryCharge,
            "Shipped",
            a.PromoCode,
            a.ShippingAddress,
            a.PaymentStatus,
            a.OrderItems,
            a.CreatedOn,
            a.ModifiedOn
        );
        a.ShouldNotBe(b);
    }

    [Fact]
    public void WithExpression_ShouldProduceUpdatedCopy()
    {
        var original = MakeSut();
        var updated = original with { OrderStatus = "Delivered", PaymentStatus = "Refunded" };
        original.OrderStatus.ShouldBe("Processing");
        updated.OrderStatus.ShouldBe("Delivered");
        updated.PaymentStatus.ShouldBe("Refunded");
        updated.OrderId.ShouldBe(original.OrderId);
    }
}

// ─────────────────────────────────────────────────────────
// AddShipmentRequest
// ─────────────────────────────────────────────────────────
public class AddShipmentRequestTests
{
    [Fact]
    public void Create_WithAllProperties_ShouldHoldCorrectValues()
    {
        var eta = DateTime.UtcNow.AddDays(5);
        var sut = new AddShipmentRequest("UPS", "1Z999AA1", eta);
        sut.Carrier.ShouldBe("UPS");
        sut.TrackingNumber.ShouldBe("1Z999AA1");
        sut.EstimatedDelivery.ShouldBe(eta);
    }

    [Fact]
    public void Create_WhenEstimatedDeliveryIsNull_ShouldBeAllowed()
    {
        var sut = new AddShipmentRequest("FedEx", "TRK123", null);
        sut.EstimatedDelivery.ShouldBeNull();
    }

    [Fact]
    public void TwoInstances_WithSameValues_ShouldBeEqual()
    {
        var eta = DateTime.UtcNow.AddDays(3);
        var a = new AddShipmentRequest("DHL", "DHL123", eta);
        var b = new AddShipmentRequest("DHL", "DHL123", eta);
        a.ShouldBe(b);
        a.GetHashCode().ShouldBe(b.GetHashCode());
    }

    [Fact]
    public void TwoInstances_WithDifferentCarrier_ShouldNotBeEqual()
    {
        var a = new AddShipmentRequest("UPS", "TRK", null);
        var b = new AddShipmentRequest("FedEx", "TRK", null);
        a.ShouldNotBe(b);
    }

    [Fact]
    public void WithExpression_ShouldProduceUpdatedCopy()
    {
        var original = new AddShipmentRequest("UPS", "OLD-TRK", null);
        var updated = original with
        {
            TrackingNumber = "NEW-TRK",
            EstimatedDelivery = DateTime.UtcNow.AddDays(7),
        };
        original.TrackingNumber.ShouldBe("OLD-TRK");
        updated.TrackingNumber.ShouldBe("NEW-TRK");
        updated.EstimatedDelivery.ShouldNotBeNull();
        updated.Carrier.ShouldBe(original.Carrier);
    }
}

// ─────────────────────────────────────────────────────────
// CreateShipmentRequest
// ─────────────────────────────────────────────────────────
public class CreateShipmentRequestTests
{
    [Fact]
    public void Create_WithAllProperties_ShouldHoldCorrectValues()
    {
        var eta = DateTime.UtcNow.AddDays(4);
        var sut = new CreateShipmentRequest("DHL", "DHL-TRK-999", eta);
        sut.Carrier.ShouldBe("DHL");
        sut.TrackingNumber.ShouldBe("DHL-TRK-999");
        sut.EstimatedDelivery.ShouldBe(eta);
    }

    [Fact]
    public void Create_WhenEstimatedDeliveryIsNull_ShouldBeAllowed()
    {
        var sut = new CreateShipmentRequest("Royal Mail", "RM123", null);
        sut.EstimatedDelivery.ShouldBeNull();
    }

    [Fact]
    public void TwoInstances_WithSameValues_ShouldBeEqual()
    {
        var eta = DateTime.UtcNow.AddDays(2);
        var a = new CreateShipmentRequest("UPS", "UP123", eta);
        var b = new CreateShipmentRequest("UPS", "UP123", eta);
        a.ShouldBe(b);
    }

    [Fact]
    public void TwoInstances_WithDifferentTrackingNumber_ShouldNotBeEqual()
    {
        var a = new CreateShipmentRequest("UPS", "TRK-A", null);
        var b = new CreateShipmentRequest("UPS", "TRK-B", null);
        a.ShouldNotBe(b);
    }
}

// ─────────────────────────────────────────────────────────
// UpdateShipmentRequest
// ─────────────────────────────────────────────────────────
public class UpdateShipmentRequestTests
{
    [Fact]
    public void Create_ShouldHoldCorrectValues()
    {
        var eta = DateTime.UtcNow.AddDays(2);
        var sut = new UpdateShipmentRequest("FedEx", "FDX-999", eta, 3);
        sut.Carrier.ShouldBe("FedEx");
        sut.TrackingNumber.ShouldBe("FDX-999");
        sut.EstimatedDelivery.ShouldBe(eta);
        sut.Status.ShouldBe(3);
    }

    [Fact]
    public void Create_WhenEstimatedDeliveryIsNull_ShouldBeAllowed()
    {
        var sut = new UpdateShipmentRequest("UPS", "TRK", null, 1);
        sut.EstimatedDelivery.ShouldBeNull();
    }

    [Fact]
    public void TwoInstances_WithSameValues_ShouldBeEqual()
    {
        var eta = DateTime.UtcNow.AddDays(3);
        var a = new UpdateShipmentRequest("DHL", "D123", eta, 2);
        var b = new UpdateShipmentRequest("DHL", "D123", eta, 2);
        a.ShouldBe(b);
        a.GetHashCode().ShouldBe(b.GetHashCode());
    }

    [Fact]
    public void TwoInstances_WithDifferentStatus_ShouldNotBeEqual()
    {
        var a = new UpdateShipmentRequest("UPS", "TRK", null, 1);
        var b = new UpdateShipmentRequest("UPS", "TRK", null, 4);
        a.ShouldNotBe(b);
    }

    [Fact]
    public void WithExpression_ShouldProduceUpdatedCopy()
    {
        var original = new UpdateShipmentRequest("UPS", "OLD", null, 1);
        var updated = original with { Status = 4, TrackingNumber = "NEW" };
        original.Status.ShouldBe(1);
        updated.Status.ShouldBe(4);
        updated.TrackingNumber.ShouldBe("NEW");
        updated.Carrier.ShouldBe(original.Carrier);
    }
}

// ─────────────────────────────────────────────────────────
// ShipmentResponse
// ─────────────────────────────────────────────────────────
public class ShipmentResponseTests
{
    [Fact]
    public void Create_ShouldHoldCorrectValues()
    {
        var sid = Guid.NewGuid();
        var eta = DateTime.UtcNow.AddDays(3);
        var created = DateTime.UtcNow;

        var sut = new ShipmentResponse(sid, "FedEx", "FDX-001", eta, "InTransit", created);

        sut.ShipmentId.ShouldBe(sid);
        sut.Carrier.ShouldBe("FedEx");
        sut.TrackingNumber.ShouldBe("FDX-001");
        sut.EstimatedDelivery.ShouldBe(eta);
        sut.Status.ShouldBe("InTransit");
        sut.CreatedOn.ShouldBe(created);
    }

    [Fact]
    public void Create_WhenEstimatedDeliveryIsNull_ShouldBeAllowed()
    {
        var sut = new ShipmentResponse(
            Guid.NewGuid(),
            "UPS",
            "TRK",
            null,
            "Pending",
            DateTime.UtcNow
        );
        sut.EstimatedDelivery.ShouldBeNull();
    }

    [Fact]
    public void TwoInstances_WithSameValues_ShouldBeEqual()
    {
        var sid = Guid.NewGuid();
        var ts = DateTime.UtcNow;
        var a = new ShipmentResponse(sid, "DHL", "D123", null, "Pending", ts);
        var b = new ShipmentResponse(sid, "DHL", "D123", null, "Pending", ts);
        a.ShouldBe(b);
        a.GetHashCode().ShouldBe(b.GetHashCode());
    }

    [Fact]
    public void TwoInstances_WithDifferentStatus_ShouldNotBeEqual()
    {
        var sid = Guid.NewGuid();
        var ts = DateTime.UtcNow;
        var a = new ShipmentResponse(sid, "UPS", "TRK", null, "Pending", ts);
        var b = new ShipmentResponse(sid, "UPS", "TRK", null, "Delivered", ts);
        a.ShouldNotBe(b);
    }

    [Fact]
    public void WithExpression_ShouldProduceUpdatedCopy()
    {
        var original = new ShipmentResponse(
            Guid.NewGuid(),
            "UPS",
            "TRK",
            null,
            "Pending",
            DateTime.UtcNow
        );
        var updated = original with { Status = "Delivered", TrackingNumber = "TRK-2" };
        original.Status.ShouldBe("Pending");
        updated.Status.ShouldBe("Delivered");
        updated.TrackingNumber.ShouldBe("TRK-2");
        updated.ShipmentId.ShouldBe(original.ShipmentId);
        updated.Carrier.ShouldBe(original.Carrier);
    }

    [Fact]
    public void ToString_ShouldContainKeyProperties()
    {
        var sut = new ShipmentResponse(
            Guid.NewGuid(),
            "DHL",
            "DHL-999",
            null,
            "Shipped",
            DateTime.UtcNow
        );
        var str = sut.ToString();
        str.ShouldContain("DHL");
        str.ShouldContain("DHL-999");
        str.ShouldContain("Shipped");
    }
}

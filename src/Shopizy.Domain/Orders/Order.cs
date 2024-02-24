using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Customers.ValueObject;
using Shopizy.Domain.Orders.Entities;
using Shopizy.Domain.Orders.Enums;
using Shopizy.Domain.Orders.ValueObjects;

namespace Shopizy.Domain.Orders;

public sealed class Order : AggregateRoot<OrderId, Guid>
{
    private readonly List<OrderItem> _orderItems  = [];
    public CustomerId CustomerId { get; }
    public BillId BillId { get; }
    public Price DeliveryCharge { get; private set; }
    public OrderStatus OrderStatus { get; private set; }
    public string PromoCode {get; private set; }
    public Address ShippingAddress { get; }
    public DateTime CreatedOn { get; private set; }
    public DateTime ModifiedOn { get; private set; }
    public IReadOnlyList<OrderItem> OrderItems => _orderItems.AsReadOnly();

    public static Order Create(
        CustomerId customerId,
        BillId billId,
        Price deliveryCharge,
        OrderStatus orderStatus,
        string promoCode,
        Address shippingAddress
    )
    {
        return new Order(
            OrderId.CreateUnique(),
            customerId,
            billId,
            deliveryCharge,
            orderStatus,
            promoCode,
            shippingAddress,
            DateTime.UtcNow,
            DateTime.UtcNow);
    }
    private Order(
        OrderId orderId,
        CustomerId customerId,
        BillId billId,
        Price deliveryCharge,
        OrderStatus orderStatus,
        string promoCode,
        Address shippingAddress,
        DateTime createdOn,
        DateTime modifiedOn
    ) : base(orderId)
    {
        CustomerId = customerId;
        BillId = billId;
        DeliveryCharge = deliveryCharge;
        OrderStatus = orderStatus;
        PromoCode = promoCode;
        ShippingAddress = shippingAddress;
        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Order() {}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

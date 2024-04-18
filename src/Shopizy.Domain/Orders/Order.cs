using shopizy.Domain.Payments.Enums;
using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Orders.Entities;
using Shopizy.Domain.Orders.Enums;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Domain.Orders;

public sealed class Order : AggregateRoot<OrderId, Guid>
{
    private readonly List<OrderItem> _orderItems = [];
    public UserId UserId { get; private set;}
    public Price DeliveryCharge { get; private set; }
    public OrderStatus OrderStatus { get; private set; }
    public string PromoCode { get; private set; }
    public Address ShippingAddress { get; private set;}
    public PaymentStatus PaymentStatus { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public DateTime ModifiedOn { get; private set; }
    public IReadOnlyList<OrderItem> OrderItems => _orderItems.AsReadOnly();

    public static Order Create(
        UserId userId,
        string promoCode,
        Price deliveryCharge,
        Address shippingAddress,
        List<OrderItem> orderItems
    )
    {
        return new Order(
            OrderId.CreateUnique(),
            userId,
            promoCode,
            deliveryCharge,
            shippingAddress,
            orderItems);
    }
    private Order(
        OrderId orderId,
        UserId userId,
        string promoCode,
        Price deliveryCharge,
        Address shippingAddress,
        List<OrderItem> orderItems
    ) : base(orderId)
    {
        UserId = userId;
        OrderStatus = OrderStatus.Submitted;
        PromoCode = promoCode;
        DeliveryCharge = deliveryCharge;
        ShippingAddress = shippingAddress;
        _orderItems = orderItems;
        PaymentStatus = PaymentStatus.Pending;
        CreatedOn = DateTime.UtcNow;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Order() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

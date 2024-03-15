using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Customers.ValueObjects;
using Shopizy.Domain.Orders.Entities;
using Shopizy.Domain.Orders.Enums;
using Shopizy.Domain.Orders.ValueObjects;

namespace Shopizy.Domain.Orders;

public sealed class Order : AggregateRoot<OrderId, Guid>
{
    private readonly List<OrderItem> _orderItems  = [];
    public CustomerId CustomerId { get; }
    public Price DeliveryCharge { get; private set; }
    public OrderStatus OrderStatus { get; private set; }
    public string PromoCode {get; private set; }
    public Address ShippingAddress { get; }
    public string PaymentStatus { get; set; }
    public DateTime CreatedOn { get; private set; }
    public DateTime ModifiedOn { get; private set; }
    public IReadOnlyList<OrderItem> OrderItems => _orderItems.AsReadOnly();

    public static Order Create(
        CustomerId customerId,
        Price deliveryCharge,
        OrderStatus orderStatus,
        string promoCode,
        Address shippingAddress,
        string paymentStatus
    )
    {
        return new Order(
            OrderId.CreateUnique(),
            customerId,
            deliveryCharge,
            orderStatus,
            promoCode,
            shippingAddress,
            paymentStatus,
            DateTime.UtcNow,
            DateTime.UtcNow);
    }
    private Order(
        OrderId orderId,
        CustomerId customerId,
        Price deliveryCharge,
        OrderStatus orderStatus,
        string promoCode,
        Address shippingAddress,
        string paymentStatus,
        DateTime createdOn,
        DateTime modifiedOn
    ) : base(orderId)
    {
        CustomerId = customerId;
        DeliveryCharge = deliveryCharge;
        OrderStatus = orderStatus;
        PromoCode = promoCode;
        ShippingAddress = shippingAddress;
        PaymentStatus = paymentStatus;
        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Order() {}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

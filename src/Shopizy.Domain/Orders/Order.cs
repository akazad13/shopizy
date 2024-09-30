using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Orders.Entities;
using Shopizy.Domain.Orders.Enums;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Payments.Enums;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Domain.Orders;

public sealed class Order : AggregateRoot<OrderId, Guid>
{
    private readonly IList<OrderItem> _orderItems = [];
    public UserId UserId { get; private set; }
    public Price DeliveryCharge { get; private set; }
    public OrderStatus OrderStatus { get; private set; }
    public string? CancellationReason { get; private set; }
    public string PromoCode { get; private set; }
    public Address ShippingAddress { get; private set; }
    public PaymentStatus PaymentStatus { get; private set; }
    public DateTime? ModifiedOn { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public IReadOnlyList<OrderItem> OrderItems => _orderItems.AsReadOnly();

    public static Order Create(
        UserId userId,
        string promoCode,
        Price deliveryCharge,
        Address shippingAddress,
        IList<OrderItem> orderItems
    )
    {
        return new Order(
            OrderId.CreateUnique(),
            userId,
            promoCode,
            deliveryCharge,
            shippingAddress,
            orderItems
        );
    }

    private Order(
        OrderId orderId,
        UserId userId,
        string promoCode,
        Price deliveryCharge,
        Address shippingAddress,
        IList<OrderItem> orderItems
    )
        : base(orderId)
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

    private Order() { }

    public void CancelOrder(string reason)
    {
        CancellationReason = reason;
        OrderStatus = OrderStatus.Cancelled;
        ModifiedOn = DateTime.UtcNow;
    }

    public Price GetTotal()
    {
        decimal totalAmount = _orderItems.Sum(item => item.TotalPrice().Amount);
        decimal totalDiscount = _orderItems.Sum(item => item.TotalDiscount().Amount);

        decimal chargeAmount = totalAmount - totalDiscount + DeliveryCharge.Amount;

        return Price.CreateNew(chargeAmount, DeliveryCharge.Currency);
    }
}

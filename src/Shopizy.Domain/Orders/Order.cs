using Shopizy.SharedKernel.Domain.Models;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Orders.Entities;
using Shopizy.Domain.Orders.Enums;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Payments.Enums;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Domain.Orders;

/// <summary>
/// Represents an order in the system.
/// </summary>
public sealed class Order : AggregateRoot<OrderId, Guid>, IAuditable
{
    private readonly IList<OrderItem> _orderItems = [];
    
    /// <summary>
    /// Gets the user ID who placed the order.
    /// </summary>
    public UserId UserId { get; private set; }
    
    /// <summary>
    /// Gets the delivery method.
    /// </summary>
    public DeliveryMethods DeliveryMethod { get; private set; }
    
    /// <summary>
    /// Gets the delivery charge.
    /// </summary>
    public Price DeliveryCharge { get; private set; }
    
    /// <summary>
    /// Gets the current order status.
    /// </summary>
    public OrderStatus OrderStatus { get; private set; }
    
    /// <summary>
    /// Gets the reason for order cancellation, if applicable.
    /// </summary>
    public string? CancellationReason { get; private set; }
    
    /// <summary>
    /// Gets the promotional code applied to the order.
    /// </summary>
    public string PromoCode { get; private set; }
    
    /// <summary>
    /// Gets the shipping address.
    /// </summary>
    public Address ShippingAddress { get; private set; }
    
    /// <summary>
    /// Gets the payment status.
    /// </summary>
    public PaymentStatus PaymentStatus { get; private set; }
    
    /// <summary>
    /// Gets the date and time when the order was last modified.
    /// </summary>
    public DateTime? ModifiedOn { get; private set; }
    
    /// <summary>
    /// Gets the date and time when the order was created.
    /// </summary>
    public DateTime CreatedOn { get; private set; }
    
    /// <summary>
    /// Gets the read-only list of order items.
    /// </summary>
    public IReadOnlyList<OrderItem> OrderItems => _orderItems.AsReadOnly();

    /// <summary>
    /// Creates a new order.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="promoCode">The promotional code.</param>
    /// <param name="deliveryMethod">The delivery method.</param>
    /// <param name="deliveryCharge">The delivery charge.</param>
    /// <param name="shippingAddress">The shipping address.</param>
    /// <param name="orderItems">The list of order items.</param>
    /// <returns>A new <see cref="Order"/> instance.</returns>
    public static Order Create(
        UserId userId,
        string promoCode,
        int deliveryMethod,
        Price deliveryCharge,
        Address shippingAddress,
        IReadOnlyList<OrderItem> orderItems
    )
    {
        var order = new Order(
            OrderId.CreateUnique(),
            userId,
            promoCode,
            deliveryMethod,
            deliveryCharge,
            shippingAddress,
            orderItems
        );

        order.AddDomainEvent(new Events.OrderCreatedDomainEvent(order));

        return order;
    }

    private Order(
        OrderId orderId,
        UserId userId,
        string promoCode,
        int deliveryMethod,
        Price deliveryCharge,
        Address shippingAddress,
        IReadOnlyList<OrderItem> orderItems
    )
        : base(orderId)
    {
        UserId = userId;
        OrderStatus = OrderStatus.Pending;
        PromoCode = promoCode;
        DeliveryMethod = (DeliveryMethods)deliveryMethod;
        DeliveryCharge = deliveryCharge;
        ShippingAddress = shippingAddress;
        _orderItems = orderItems.ToList();
        PaymentStatus = PaymentStatus.Pending;
    }

    private Order() { }

    /// <summary>
    /// Cancels the order with a specified reason.
    /// </summary>
    /// <param name="reason">The reason for cancellation.</param>
    public void CancelOrder(string reason)
    {
        CancellationReason = reason;
        OrderStatus = OrderStatus.Cancelled;

        this.AddDomainEvent(new Events.OrderCancelledDomainEvent(this));
    }

    /// <summary>
    /// Calculates the total order amount including delivery charges and discounts.
    /// </summary>
    /// <returns>The total <see cref="Price"/> of the order.</returns>
    public Price GetTotal()
    {
        decimal totalAmount = _orderItems.Sum(item => item.TotalPrice().Amount);
        decimal totalDiscount = _orderItems.Sum(item => item.TotalDiscount().Amount);

        decimal chargeAmount = totalAmount - totalDiscount + DeliveryCharge.Amount;

        return Price.CreateNew(chargeAmount, DeliveryCharge.Currency);
    }

    /// <summary>
    /// Updates the payment status of the order.
    /// </summary>
    /// <param name="status">The new payment status.</param>
    public void UpdatePaymentStatus(PaymentStatus status)
    {
        PaymentStatus = status;
    }

    /// <summary>
    /// Updates the order status.
    /// </summary>
    /// <param name="status">The new order status.</param>
    public void UpdateOrderStatus(OrderStatus status)
    {
        OrderStatus = status;
    }
}

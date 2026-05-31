using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Orders.Entities;
using Shopizy.Domain.Orders.Enums;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Payments.Enums;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Domain.Models;

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
    public required UserId UserId { get; init; }

    /// <summary>
    /// Gets the delivery method.
    /// </summary>
    public required DeliveryMethods DeliveryMethod { get; init; }

    /// <summary>
    /// Gets the delivery charge.
    /// </summary>
    public required Price DeliveryCharge { get; init; }

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
    public required string PromoCode { get; init; }

    /// <summary>
    /// Gets the shipping address.
    /// </summary>
    public required Address ShippingAddress { get; init; }

    /// <summary>
    /// Gets the payment status.
    /// </summary>
    public PaymentStatus PaymentStatus { get; private set; }

    /// <summary>
    /// Gets the date and time when the order was last modified.
    /// </summary>
    public DateTime? ModifiedOn { get; }

    /// <summary>
    /// Gets the date and time when the order was created.
    /// </summary>
    public DateTime CreatedOn { get; }

    /// <summary>
    /// Gets the read-only list of order items.
    /// </summary>
    public IReadOnlyList<OrderItem> OrderItems => _orderItems.AsReadOnly();

    /// <summary>
    /// Gets the shipment associated with this order.
    /// </summary>
    public Shipment? Shipment { get; private set; }

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
        var order = new Order
        {
            Id = OrderId.CreateUnique(),
            UserId = userId,
            PromoCode = promoCode,
            DeliveryMethod = (DeliveryMethods)deliveryMethod,
            DeliveryCharge = deliveryCharge,
            ShippingAddress = shippingAddress,
            OrderStatus = OrderStatus.Pending,
            PaymentStatus = PaymentStatus.Pending,
        };

        foreach (var item in orderItems)
        {
            order._orderItems.Add(item);
        }

        order.AddDomainEvent(new Events.OrderCreatedDomainEvent(order));

        return order;
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

        AddDomainEvent(new Events.OrderCancelledDomainEvent(this));
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
    public void UpdatePaymentStatus(PaymentStatus status) => PaymentStatus = status;

    /// <summary>
    /// Updates the order status.
    /// </summary>
    /// <param name="status">The new order status.</param>
    public void UpdateOrderStatus(OrderStatus status) => OrderStatus = status;

    /// <summary>
    /// Marks the order payment as complete, transitions to Processing status,
    /// and raises a <see cref="Events.PaymentCompletedDomainEvent"/>.
    /// </summary>
    /// <param name="customerId">The Stripe customer ID associated with this payment.</param>
    public void CompletePayment(string customerId)
    {
        PaymentStatus = PaymentStatus.Payed;
        OrderStatus = OrderStatus.Processing;

        AddDomainEvent(new Events.PaymentCompletedDomainEvent(Id, UserId, customerId));
    }

    /// <summary>
    /// Adds a shipment to the order.
    /// </summary>
    /// <param name="carrier"></param>
    /// <param name="trackingNumber"></param>
    /// <param name="estimatedDelivery"></param>
    public DomainResult<Shipment> AddShipment(
        string carrier,
        string trackingNumber,
        DateTime? estimatedDelivery
    )
    {
        if (Shipment is not null)
        {
            return DomainError.Conflict("Order.ShipmentExists", "Order already has a shipment.");
        }

        Shipment = Shipment.Create(carrier, trackingNumber, estimatedDelivery);
        return Shipment;
    }

    /// <summary>
    /// Updates the shipment associated with the order.
    /// </summary>
    /// <param name="carrier"></param>
    /// <param name="trackingNumber"></param>
    /// <param name="estimatedDelivery"></param>
    /// <param name="status"></param>
    public DomainResult<bool> UpdateShipment(
        string carrier,
        string trackingNumber,
        DateTime? estimatedDelivery,
        ShipmentStatus status
    )
    {
        if (Shipment is null)
        {
            return DomainError.NotFound("Order.ShipmentNotFound", "Order has no shipment.");
        }

        Shipment.Update(carrier, trackingNumber, estimatedDelivery, status);
        return true;
    }
}

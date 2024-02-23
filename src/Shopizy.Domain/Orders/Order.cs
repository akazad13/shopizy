using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Customers.ValueObject;
using Shopizy.Domain.Orders.Entities;
using Shopizy.Domain.Orders.Enums;
using Shopizy.Domain.Orders.ValueObjects;

namespace Shopizy.Domain.Orders;

public sealed class Order : AggregateRoot<OrderId, Guid>
{
    public CustomerId CustomerId { get; }
    // public decimal TotalAmount { get; private set; }
    // public decimal Discount { get; private set; }
    public decimal DeliveryCharge { get; private set; }
    // public decimal FinalAmount { get; private set; }
    // public Currency Currency { get; private set; }
    public OrderStatus OrderStatus { get; private set; }
    public string PromoCode {get; private set; }
    public DateTime CreatedDateTime { get; private set; }
    public DateTime UpdatedDateTime { get; private set; }
    public readonly List<OrderItem> _orderItems  = [];
    // public IReadOnlyList<OrderItem> OrderItems => _orderItems.AsReadOnly();
    public Address ShippingAddress { get; }
}

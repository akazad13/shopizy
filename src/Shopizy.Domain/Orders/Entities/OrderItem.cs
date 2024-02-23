using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Orders.ValueObjects;

namespace Shopizy.Domain.Orders.Entities;

public sealed class OrderItem : Entity<OrderItemId>
{
    public string Name { get; private set; }
    public string PictureUrl { get; private set; }
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal Discount { get; private set; }

    public static OrderItem Create(
        string name,
        string pictureUrl,
        decimal unitPrice,
        int quantity,
        decimal discount
    )
    {
        return new OrderItem(
            OrderItemId.CreateUnique(),
            name,
            pictureUrl,
            unitPrice,
            quantity,
            discount
        );
    }

    private OrderItem(
        OrderItemId id,
        string name,
        string pictureUrl,
        decimal unitPrice,
        int quantity,
        decimal discount
    ) : base(id)
    {
        Name = name;
        PictureUrl = pictureUrl;
        UnitPrice = unitPrice;
        Quantity = quantity;
        Discount = discount;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private OrderItem() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Orders.ValueObjects;

namespace Shopizy.Domain.Orders.Entities;

public sealed class OrderItem : Entity<OrderItemId>
{
    public string Name { get; private set; }
    public string PictureUrl { get; private set; }
    public Price UnitPrice { get; private set; }
    public string Color { get; private set; }
    public string Size { get; private set; }
    public int Quantity { get; private set; }
    public decimal Discount { get; private set; }

    public static OrderItem Create(
        string name,
        string pictureUrl,
        Price unitPrice,
        int quantity,
        string color,
        string size,
        decimal? discount
    )
    {
        return new OrderItem(
            OrderItemId.CreateUnique(),
            name,
            pictureUrl,
            unitPrice,
            quantity,
            color,
            size,
            discount
        );
    }

    private OrderItem(
        OrderItemId orderItemId,
        string name,
        string pictureUrl,
        Price unitPrice,
        int quantity,
        string color,
        string size,
        decimal? discount
    )
        : base(orderItemId)
    {
        Name = name;
        PictureUrl = pictureUrl;
        UnitPrice = unitPrice;
        Quantity = quantity;
        Color = color;
        Size = size;
        Discount = discount ?? 0;
    }

    private OrderItem() { }

    public Price TotalPrice()
    {
        return Price.CreateNew(UnitPrice.Amount * Quantity, UnitPrice.Currency);
    }

    public Price TotalDiscount()
    {
        return Price.CreateNew(UnitPrice.Amount * (Discount / 100) * Quantity, UnitPrice.Currency);
    }
}

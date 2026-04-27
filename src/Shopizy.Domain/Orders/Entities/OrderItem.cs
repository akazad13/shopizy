using Shopizy.SharedKernel.Domain.Models;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Domain.Orders.Entities;

public sealed class OrderItem : Entity<OrderItemId>
{
    public ProductId ProductId { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string PictureUrl { get; private set; } = null!;
    public Price UnitPrice { get; private set; } = null!;
    public string Color { get; private set; } = null!;
    public string Size { get; private set; } = null!;
    public int Quantity { get; private set; }
    public decimal Discount { get; private set; }

    public static OrderItem Create(
        ProductId productId,
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
            productId,
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
        ProductId productId,
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
        ProductId = productId;
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

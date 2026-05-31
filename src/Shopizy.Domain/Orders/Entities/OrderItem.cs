using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Orders.Entities;

public sealed class OrderItem : Entity<OrderItemId>
{
    public ProductId ProductId { get; } = null!;
    public string Name { get; } = null!;
    public string PictureUrl { get; } = null!;
    public Price UnitPrice { get; } = null!;
    public string Color { get; } = null!;
    public string Size { get; } = null!;
    public int Quantity { get; }
    public decimal Discount { get; }

    public static OrderItem Create(
        ProductId productId,
        string name,
        string pictureUrl,
        Price unitPrice,
        int quantity,
        string color,
        string size,
        decimal? discount
    ) =>
        new(
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

    public Price TotalPrice() => Price.CreateNew(UnitPrice.Amount * Quantity, UnitPrice.Currency);

    public Price TotalDiscount() =>
        Price.CreateNew(UnitPrice.Amount * (Discount / 100) * Quantity, UnitPrice.Currency);
}

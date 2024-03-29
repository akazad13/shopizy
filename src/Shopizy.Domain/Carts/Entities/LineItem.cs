using Shopizy.Domain.Carts.ValueObjects;
using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Domain.Carts.Entities;

public sealed class LineItem : Entity<LineItemId>
{
    public Product Product { get; private set; } = null!;
    public ProductId ProductId { get; private set; }
    public int Quantity { get; private set; }

    public static LineItem Create(ProductId productId, int quantity)
    {
        return new LineItem(LineItemId.CreateUnique(), productId, quantity);
    }

    private LineItem(LineItemId lineItemId, ProductId productId, int quantity) : base(lineItemId)
    {
        ProductId = productId;
        Quantity = quantity;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private LineItem() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

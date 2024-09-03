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

    public static LineItem Create(ProductId productId)
    {
        return new LineItem(LineItemId.CreateUnique(), productId);
    }

    public void UpdateQuantity(int quantity)
    {
        Quantity = quantity;
    }

    private LineItem(LineItemId lineItemId, ProductId productId) : base(lineItemId)
    {
        ProductId = productId;
        Quantity = 1;
    }

    private LineItem() { }
}

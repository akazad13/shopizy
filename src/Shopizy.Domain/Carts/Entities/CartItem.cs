using Shopizy.Domain.Carts.ValueObjects;
using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Domain.Carts.Entities;

public sealed class CartItem : Entity<CartItemId>
{
    public Product Product { get; private set; } = null!;
    public ProductId ProductId { get; private set; }
    public string Color { get; private set; }
    public string Size { get; private set; }
    public int Quantity { get; private set; }

    public static CartItem Create(ProductId productId, string color, string size)
    {
        return new CartItem(CartItemId.CreateUnique(), productId, color, size);
    }

    public void UpdateQuantity(int quantity)
    {
        Quantity = quantity;
    }

    private CartItem(CartItemId CartItemId, ProductId productId, string color, string size)
        : base(CartItemId)
    {
        ProductId = productId;
        Color = color;
        Size = size;
        Quantity = 1;
    }

    private CartItem() { }
}

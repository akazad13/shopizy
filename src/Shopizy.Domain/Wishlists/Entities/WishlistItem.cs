using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Wishlists.ValueObjects;
using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Wishlists.Entities;

public sealed class WishlistItem : Entity<WishlistItemId>
{
    public ProductId ProductId { get; } = null!;

    public static WishlistItem Create(ProductId productId) =>
        new(WishlistItemId.CreateUnique(), productId);

    private WishlistItem(WishlistItemId id, ProductId productId)
        : base(id)
    {
        ProductId = productId;
    }

    private WishlistItem() { }
}

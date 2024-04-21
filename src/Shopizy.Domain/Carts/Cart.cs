using Shopizy.Domain.Carts.Entities;
using Shopizy.Domain.Carts.ValueObjects;
using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Domain.Carts;

public sealed class Cart : AggregateRoot<CartId, Guid>
{
    private readonly List<LineItem> _lineItems = [];
    public UserId UserId { get; }
    public DateTime CreatedOn { get; private set; }
    public DateTime? ModifiedOn { get; private set; }
    public IReadOnlyList<LineItem> LineItems => _lineItems.AsReadOnly();

    public static Cart Create(
        UserId userId
    )
    {
        return new Cart(
            CartId.CreateUnique(),
            userId);
    }

    public void AddLineItem(LineItem lineItem)
    {
        _lineItems.Add(lineItem);
    }

    public void RemoveLineItem(LineItem lineItem)
    {
        _lineItems.Remove(lineItem);
    }

    public void UpdateLineItem(ProductId productId, int quantity)
    {
        _lineItems.Find(li => li.ProductId == productId)?.UpdateQuantity(quantity);
    }

    private Cart(
        CartId cartId,
        UserId userId
    ) : base(cartId)
    {
        UserId = userId;
        CreatedOn = DateTime.UtcNow;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Cart() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

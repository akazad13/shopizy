using Shopizy.Domain.Carts.Entities;
using Shopizy.Domain.Carts.ValueObjects;
using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Customers.ValueObjects;

namespace Shopizy.Domain.Carts;

public sealed class Cart : AggregateRoot<CartId, Guid>
{
    private readonly List<LineItem> _lineItems = [];
    public CustomerId CustomerId { get; }
    public DateTime CreatedOn { get; private set; }
    public DateTime ModifiedOn { get; private set; }
    public IReadOnlyList<LineItem> LineItems => _lineItems.AsReadOnly();

    public static Cart Create(
        CustomerId customerId
    )
    {
        return new Cart(
            CartId.CreateUnique(),
            customerId,
            DateTime.UtcNow,
            DateTime.UtcNow);
    }

    public void AddLineItem(LineItem lineItem)
    {
        _lineItems.Add(lineItem);
    }

    public void RemoveLineItem(LineItem lineItem)
    {
        _lineItems.Remove(lineItem);
    }

    private Cart(
        CartId cartId,
        CustomerId customerId,
        DateTime createdOn,
        DateTime modifiedOn
    ) : base(cartId)
    {
        CustomerId = customerId;
        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Cart() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

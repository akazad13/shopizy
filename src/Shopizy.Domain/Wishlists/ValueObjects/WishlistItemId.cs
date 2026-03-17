using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Wishlists.ValueObjects;

public sealed class WishlistItemId : AggregateRootId<Guid>
{
    public override Guid Value { get; protected set; }

    private WishlistItemId(Guid value)
    {
        Value = value;
    }

    public static WishlistItemId CreateUnique()
    {
        return new(Guid.NewGuid());
    }

    public static WishlistItemId Create(Guid value)
    {
        return new(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

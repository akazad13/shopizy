using Shopizy.Domain.Common.Models;

namespace Shopizy.Domain.Carts.ValueObjects;

public sealed class CartItemId : AggregateRootId<Guid>
{
    public override Guid Value { get; protected set; }

    private CartItemId(Guid value)
    {
        Value = value;
    }

    public static CartItemId CreateUnique()
    {
        return new(Guid.NewGuid());
    }

    public static CartItemId Create(Guid value)
    {
        return new(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

using Shopizy.Domain.Common.Models;

namespace Shopizy.Domain.Carts.ValueObjects;

public sealed class CartId : AggregateRootId<Guid>
{
    public override Guid Value { get; protected set; }

    private CartId(Guid value)
    {
        Value = value;
    }

    public static CartId CreateUnique()
    {
        return new(Guid.NewGuid());
    }

    public static CartId Create(Guid value)
    {
        return new(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

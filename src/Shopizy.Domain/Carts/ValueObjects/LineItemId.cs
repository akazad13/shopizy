using Shopizy.Domain.Common.Models;

namespace Shopizy.Domain.Carts.ValueObjects;

public sealed class LineItemId : AggregateRootId<Guid>
{
    public override Guid Value { get; protected set; }

    private LineItemId(Guid value)
    {
        Value = value;
    }

    public static LineItemId CreateUnique()
    {
        return new(Guid.NewGuid());
    }

    public static LineItemId Create(Guid value)
    {
        return new(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

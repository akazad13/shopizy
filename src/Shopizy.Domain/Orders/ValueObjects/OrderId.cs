using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Orders.ValueObjects;

public sealed class OrderId : AggregateRootId<Guid>
{
    public override Guid Value { get; protected set; }

    private OrderId(Guid value)
    {
        Value = value;
    }

    public static OrderId CreateUnique()
    {
        return new(Guid.NewGuid());
    }

    public static OrderId Create(Guid value)
    {
        return new(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

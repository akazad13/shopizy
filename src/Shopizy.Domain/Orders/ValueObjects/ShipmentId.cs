using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Orders.ValueObjects;

public sealed class ShipmentId : AggregateRootId<Guid>
{
    public override Guid Value { get; protected set; }

    private ShipmentId(Guid value)
    {
        Value = value;
    }

    public static ShipmentId CreateUnique() => new(Guid.NewGuid());
    public static ShipmentId Create(Guid value) => new(value);

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

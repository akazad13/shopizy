using Shopizy.Domain.Common.Models;

namespace Shopizy.Domain.PromoCodes.ValueObjects;

public sealed class PromoCodeId : AggregateRootId<Guid>
{
    public override Guid Value { get; protected set; }

    private PromoCodeId(Guid value)
    {
        Value = value;
    }

    public static PromoCodeId CreateUnique()
    {
        return new(Guid.NewGuid());
    }

    public static PromoCodeId Create(Guid value)
    {
        return new(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

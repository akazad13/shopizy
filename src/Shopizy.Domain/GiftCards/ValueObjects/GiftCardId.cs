using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.GiftCards.ValueObjects;

public sealed class GiftCardId : AggregateRootId<Guid>
{
    public override Guid Value { get; protected set; }

    private GiftCardId(Guid value)
    {
        Value = value;
    }

    public static GiftCardId CreateUnique()
    {
        return new(Guid.NewGuid());
    }

    public static GiftCardId Create(Guid value)
    {
        return new(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

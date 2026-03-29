using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.LoyaltyAccounts.ValueObjects;

public sealed class LoyaltyTransactionId : AggregateRootId<Guid>
{
    public override Guid Value { get; protected set; }

    private LoyaltyTransactionId(Guid value)
    {
        Value = value;
    }

    public static LoyaltyTransactionId CreateUnique()
    {
        return new(Guid.NewGuid());
    }

    public static LoyaltyTransactionId Create(Guid value)
    {
        return new(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

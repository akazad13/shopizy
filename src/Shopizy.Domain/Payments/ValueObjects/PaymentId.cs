using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Payments.ValueObjects;

public sealed class PaymentId : AggregateRootId<Guid>
{
    public override Guid Value { get; protected set; }

    private PaymentId(Guid value)
    {
        Value = value;
    }

    public static PaymentId CreateUnique()
    {
        return new(Guid.NewGuid());
    }

    public static PaymentId Create(Guid value)
    {
        return new(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

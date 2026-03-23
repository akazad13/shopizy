using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.ProductQuestions.ValueObjects;

public sealed class ProductAnswerId : AggregateRootId<Guid>
{
    public override Guid Value { get; protected set; }

    private ProductAnswerId(Guid value)
    {
        Value = value;
    }

    public static ProductAnswerId CreateUnique()
    {
        return new(Guid.NewGuid());
    }

    public static ProductAnswerId Create(Guid value)
    {
        return new(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

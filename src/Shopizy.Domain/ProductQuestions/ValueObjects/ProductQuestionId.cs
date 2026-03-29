using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.ProductQuestions.ValueObjects;

public sealed class ProductQuestionId : AggregateRootId<Guid>
{
    public override Guid Value { get; protected set; }

    private ProductQuestionId(Guid value)
    {
        Value = value;
    }

    public static ProductQuestionId CreateUnique()
    {
        return new(Guid.NewGuid());
    }

    public static ProductQuestionId Create(Guid value)
    {
        return new(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

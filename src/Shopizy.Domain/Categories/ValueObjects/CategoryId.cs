using Shopizy.Domain.Common.Models;

namespace Shopizy.Domain.Categories.ValueObjects;

public sealed class CategoryId : AggregateRootId<Guid>
{
    public override Guid Value { get; protected set; }

    private CategoryId(Guid value)
    {
        Value = value;
    }

    public static CategoryId CreateUnique()
    {
        return new(Guid.NewGuid());
    }

    public static CategoryId Create(Guid value)
    {
        return new(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

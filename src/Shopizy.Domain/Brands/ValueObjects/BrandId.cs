using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Brands.ValueObjects;

public sealed class BrandId : AggregateRootId<Guid>
{
    public override Guid Value { get; protected set; }

    private BrandId(Guid value)
    {
        Value = value;
    }

    public static BrandId CreateUnique()
    {
        return new(Guid.NewGuid());
    }

    public static BrandId Create(Guid value)
    {
        return new(value);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
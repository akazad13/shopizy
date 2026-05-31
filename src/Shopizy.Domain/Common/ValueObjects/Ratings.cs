using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Common.ValueObjects;

public sealed class Rating : ValueObject
{
    public decimal Value { get; }

    private Rating(decimal value)
    {
        Value = value;
    }

    public static Rating CreateNew(decimal value) => new(value);

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

using Shopizy.Domain.Common.Enums;
using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Common.ValueObjects;

public sealed class Price : ValueObject
{
    public decimal Amount { get; }
    public Currency Currency { get; }

    private Price(decimal Amount, Currency Currency)
    {
        this.Amount = Amount;
        this.Currency = Currency;
    }

    public static Price CreateNew(decimal amount, Currency currency) => new(amount, currency);

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}

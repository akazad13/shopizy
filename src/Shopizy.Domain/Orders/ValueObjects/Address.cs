using Shopizy.Domain.Common.Models;

namespace Shopizy.Domain.Orders.ValueObjects;

public sealed class Address : ValueObject
{
    public string Line { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string Country { get; private set; }
    public string ZipCode { get; private set; }

    private Address(string line, string city, string state, string country, string zipCode)
    {
        Line = line;
        City = city;
        State = state;
        Country = country;
        ZipCode = zipCode;
    }

    public static Address CreateNew(
        string line,
        string city,
        string state,
        string country,
        string zipCode
    )
    {
        return new(line, city, state, country, zipCode);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Line;
        yield return City;
        yield return State;
        yield return Country;
        yield return ZipCode;
    }
}

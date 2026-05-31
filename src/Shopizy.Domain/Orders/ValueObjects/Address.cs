using System.Text.Json.Serialization;
using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Orders.ValueObjects;

public sealed class Address : ValueObject
{
    public string Street { get; }
    public string City { get; }
    public string State { get; }
    public string Country { get; }
    public string ZipCode { get; }

    [JsonConstructor]
    private Address(string street, string city, string state, string country, string zipCode)
    {
        Street = street;
        City = city;
        State = state;
        Country = country;
        ZipCode = zipCode;
    }

    public static Address CreateNew(
        string street,
        string city,
        string state,
        string country,
        string zipCode
    ) => new(street, city, state, country, zipCode);

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return Country;
        yield return ZipCode;
    }
}

using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Users.Entities;

public sealed class UserAddress : Entity<UserAddressId>
{
    public string Street { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string Country { get; private set; }
    public string ZipCode { get; private set; }
    public bool IsDefault { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public DateTime? ModifiedOn { get; private set; }

    public static UserAddress Create(
        string street,
        string city,
        string state,
        string country,
        string zipCode,
        bool isDefault
    )
    {
        return new UserAddress(
            UserAddressId.CreateUnique(),
            street,
            city,
            state,
            country,
            zipCode,
            isDefault
        );
    }

    public void Update(string street, string city, string state, string country, string zipCode)
    {
        Street = street;
        City = city;
        State = state;
        Country = country;
        ZipCode = zipCode;
    }

    public void SetDefault(bool isDefault)
    {
        IsDefault = isDefault;
    }

    private UserAddress(
        UserAddressId id,
        string street,
        string city,
        string state,
        string country,
        string zipCode,
        bool isDefault
    )
        : base(id)
    {
        Street = street;
        City = city;
        State = state;
        Country = country;
        ZipCode = zipCode;
        IsDefault = isDefault;
        CreatedOn = DateTime.UtcNow;
    }

    private UserAddress() { }
}

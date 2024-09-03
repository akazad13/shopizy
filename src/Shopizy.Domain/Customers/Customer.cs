using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Customers.ValueObjects;
using Shopizy.Domain.Orders.ValueObjects;

namespace Shopizy.Domain.Customers;

public sealed class Customer : AggregateRoot<CustomerId, Guid>
{
    public string? ProfileImageUrl { get; private set; }

    // public UserId UserId { get; }
    // public User User { get; } = null!;
    public Address Address { get; set; }
    public DateTime CreatedOn { get; private set; }
    public DateTime ModifiedOn { get; private set; }

    public static Customer Create(string? profileImageUrl, Address address)
    {
        return new Customer(
            CustomerId.CreateUnique(),
            profileImageUrl,
            // userId,
            address,
            DateTime.UtcNow,
            DateTime.UtcNow
        );
    }

    private Customer(
        CustomerId customerId,
        string? profileImageUrl,
        // UserId userId,
        Address address,
        DateTime createdOn,
        DateTime modifiedOn
    ) : base(customerId)
    {
        ProfileImageUrl = profileImageUrl;
        // UserId = userId;
        Address = address;
        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
    }

    private Customer() { }
}

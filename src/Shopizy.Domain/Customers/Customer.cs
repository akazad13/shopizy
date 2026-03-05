using Shopizy.SharedKernel.Domain.Models;
using Shopizy.Domain.Customers.ValueObjects;
using Shopizy.Domain.Orders.ValueObjects;

namespace Shopizy.Domain.Customers;

/// <summary>
/// Represents a customer in the system.
/// </summary>
public sealed class Customer : AggregateRoot<CustomerId, Guid>, IAuditable
{
    /// <summary>
    /// Gets the customer's profile image URL.
    /// </summary>
    public string? ProfileImageUrl { get; private set; }

    /// <summary>
    /// Gets or sets the customer's address.
    /// </summary>
    public Address Address { get; set; }
    
    /// <summary>
    /// Gets the date and time when the customer was created.
    /// </summary>
    public DateTime CreatedOn { get; private set; }
    
    /// <summary>
    /// Gets the date and time when the customer was last modified.
    /// </summary>
    public DateTime? ModifiedOn { get; private set; }

    /// <summary>
    /// Creates a new customer.
    /// </summary>
    /// <param name="profileImageUrl">The profile image URL.</param>
    /// <param name="address">The customer's address.</param>
    /// <returns>A new <see cref="Customer"/> instance.</returns>
    public static Customer Create(string? profileImageUrl, Address address)
    {
        return new Customer(
            CustomerId.CreateUnique(),
            profileImageUrl,
            address
        );
    }

    private Customer(
        CustomerId customerId,
        string? profileImageUrl,
        Address address
    ) : base(customerId)
    {
        ProfileImageUrl = profileImageUrl;
        Address = address;
    }

    private Customer() { }
}

using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Permissions.ValueObjects;
using Shopizy.Domain.ProductReviews.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Domain.Users;

/// <summary>
/// Represents a user in the system.
/// </summary>
public sealed class User : AggregateRoot<UserId, Guid>, IAuditable
{
    private readonly List<OrderId> _orderIds = [];
    private readonly List<ProductReviewId> _productReviewIds = [];
    private readonly List<PermissionId> _permissionIds = [];
    
    /// <summary>
    /// Gets the user's first name.
    /// </summary>
    public string FirstName { get; private set; }
    
    /// <summary>
    /// Gets the user's last name.
    /// </summary>
    public string LastName { get; private set; }
    
    /// <summary>
    /// Gets the user's email address.
    /// </summary>
    public string Email { get; private set; }
    
    /// <summary>
    /// Gets the user's profile image URL.
    /// </summary>
    public string? ProfileImageUrl { get; }
    
    /// <summary>
    /// Gets the user's phone number.
    /// </summary>
    public string Phone { get; private set; }
    
    /// <summary>
    /// Gets the user's hashed password.
    /// </summary>
    public string? Password { get; private set; }
    
    /// <summary>
    /// Gets the user's customer ID for payment processing.
    /// </summary>
    public string? CustomerId { get; private set; } = null;
    
    /// <summary>
    /// Gets the user's address.
    /// </summary>
    public Address? Address { get; private set; }
    
    /// <summary>
    /// Gets the date and time when the user was created.
    /// </summary>
    public DateTime CreatedOn { get; private set; }
    
    /// <summary>
    /// Gets the date and time when the user was last modified.
    /// </summary>
    public DateTime? ModifiedOn { get; private set; }

    /// <summary>
    /// Gets the read-only list of order IDs associated with this user.
    /// </summary>
    public IReadOnlyList<OrderId> OrderIds => _orderIds.AsReadOnly();
    
    /// <summary>
    /// Gets the read-only list of product review IDs created by this user.
    /// </summary>
    public IReadOnlyList<ProductReviewId> ProductReviewIds => _productReviewIds.AsReadOnly();
    
    /// <summary>
    /// Gets the read-only list of permission IDs assigned to this user.
    /// </summary>
    public IReadOnlyList<PermissionId> PermissionIds => _permissionIds.AsReadOnly();

    /// <summary>
    /// Creates a new user instance.
    /// </summary>
    /// <param name="firstName">The user's first name.</param>
    /// <param name="lastName">The user's last name.</param>
    /// <param name="email">The user's email address.</param>
    /// <param name="password">The user's hashed password.</param>
    /// <param name="permissionIds">The list of permission IDs to assign to the user.</param>
    /// <returns>A new <see cref="User"/> instance.</returns>
    public static User Create(
        string firstName,
        string lastName,
        string email,
        string? password,
        IList<PermissionId> permissionIds
    )
    {
        var user = new User(UserId.CreateUnique(), firstName, lastName, email, password, permissionIds);
        user.AddDomainEvent(new Events.UserRegisteredDomainEvent(user));
        
        return user;
    }

    private User() { }

    private User(
        UserId userId,
        string firstName,
        string lastName,
        string email,
        string? password,
        IList<PermissionId> permissionIds
    )
        : base(userId)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Password = password;
        _permissionIds = permissionIds.ToList();
    }

    /// <summary>
    /// Updates the user's address information.
    /// </summary>
    /// <param name="street">The street address.</param>
    /// <param name="city">The city.</param>
    /// <param name="state">The state or province.</param>
    /// <param name="country">The country.</param>
    /// <param name="zipCode">The postal/ZIP code.</param>
    public void UpdateAddress(
        string street,
        string city,
        string state,
        string country,
        string zipCode
    )
    {
        Address = Address.CreateNew(street, city, state, country, zipCode);
    }

    /// <summary>
    /// Updates the user's password.
    /// </summary>
    /// <param name="password">The new hashed password.</param>
    public void UpdatePassword(string password)
    {
        Password = password;
    }

    /// <summary>
    /// Updates the user's profile information.
    /// </summary>
    /// <param name="firstName">The user's first name.</param>
    /// <param name="lastName">The user's last name.</param>
    /// <param name="email">The user's email address.</param>
    /// <param name="phone">The user's phone number.</param>
    /// <param name="street">The street address.</param>
    /// <param name="city">The city.</param>
    /// <param name="state">The state or province.</param>
    /// <param name="country">The country.</param>
    /// <param name="zipCode">The postal/ZIP code.</param>
    public void Update(
        string firstName,
        string lastName,
        string email,
        string? phone,
        string street,
        string city,
        string state,
        string country,
        string zipCode
    )
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone ?? string.Empty;
        Address = Address.CreateNew(street, city, state, country, zipCode);
    }

    /// <summary>
    /// Updates the user's customer ID for payment processing.
    /// </summary>
    /// <param name="customerId">The customer ID from the payment provider.</param>
    public void UpdateCustomerId(string customerId)
    {
        CustomerId = customerId;
    }
}

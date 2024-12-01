using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Orders;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.ProductReviews;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Domain.Users;

public sealed class User : AggregateRoot<UserId, Guid>
{
    private readonly List<Order> _orders = [];
    private readonly List<ProductReview> _productReviews = [];
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    public string Email { get; private set; }
    public string? ProfileImageUrl { get; }
    public string Phone { get; private set; }
    public string? Password { get; private set; }
    public string? CustomerId { get; private set; } = null;
    public Address? Address { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public DateTime? ModifiedOn { get; private set; }

    public IReadOnlyList<Order> Orders => _orders.AsReadOnly();
    public IReadOnlyList<ProductReview> ProductReviews => _productReviews.AsReadOnly();

    public static User Create(string firstName, string lastName, string phone, string? password)
    {
        return new(UserId.CreateUnique(), firstName, lastName, phone, password);
    }

    private User() { }

    private User(UserId userId, string firstName, string lastName, string phone, string? password)
        : base(userId)
    {
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        Password = password;
        CreatedOn = DateTime.UtcNow;
    }

    public void UpdateAddress(Address address)
    {
        Address = address;
        ModifiedOn = DateTime.UtcNow;
    }

    public void UpdatePassword(string password)
    {
        Password = password;
        ModifiedOn = DateTime.UtcNow;
    }

    public void UpdateEmail(string email)
    {
        Email = email;
        ModifiedOn = DateTime.UtcNow;
    }

    public void UpdateCustomerId(string customerId)
    {
        CustomerId = customerId;
        ModifiedOn = DateTime.UtcNow;
    }
}

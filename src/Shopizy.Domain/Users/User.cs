using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Orders;
using Shopizy.Domain.ProductReviews;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Domain.Users;

public sealed class User : AggregateRoot<UserId, Guid>
{
    private readonly List<Order> _orders = [];
    private readonly List<ProductReview> _productReviews = [];
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Phone { get; private set; }
    public string? Password { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public DateTime ModifiedOn { get; private set; }

    public IReadOnlyList<Order> Orders => _orders.AsReadOnly();
    public IReadOnlyList<ProductReview> ProductReviews => _productReviews.AsReadOnly();

    public static User Create(string firstName, string lastName, string phone, string? password)
    {
        return new(
            UserId.CreateUnique(),
            firstName,
            lastName,
            phone,
            password,
            DateTime.UtcNow,
            DateTime.UtcNow
        );
    }

    private User(
        UserId userId,
        string firstName,
        string lastName,
        string phone,
        string? password,
        DateTime createdOn,
        DateTime modifiedOn
    ) : base(userId)
    {
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        Password = password;
        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private User() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

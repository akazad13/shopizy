using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Permissions.ValueObjects;
using Shopizy.Domain.ProductReviews.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Domain.Users;

public sealed class User : AggregateRoot<UserId, Guid>
{
    private readonly IList<OrderId> _orderIds = [];
    private readonly IList<ProductReviewId> _productReviewIds = [];
    private readonly IList<PermissionId> _permissionIds = [];
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

    public IReadOnlyList<OrderId> OrderIds => _orderIds.AsReadOnly();
    public IReadOnlyList<ProductReviewId> ProductReviewIds => _productReviewIds.AsReadOnly();
    public IReadOnlyList<PermissionId> PermissionIds => _permissionIds.AsReadOnly();

    public static User Create(
        string firstName,
        string lastName,
        string email,
        string? password,
        IList<PermissionId> permissionIds
    )
    {
        return new(UserId.CreateUnique(), firstName, lastName, email, password, permissionIds);
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

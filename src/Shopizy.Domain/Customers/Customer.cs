using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Customers.ValueObject;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.ProductReviews.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Domain.Customers;

public sealed class Customer : AggregateRoot<CustomerId, Guid>
{
    private readonly List<OrderId> _orderIds = [];
    private readonly List<ProductReviewId> _productReviewIds = [];
    public string ProfileImageUrl { get; private set; }
    public UserId UserId { get; }
    public Address Address { get; set; }
    public IReadOnlyList<OrderId> OrderIds => _orderIds.AsReadOnly();
    public IReadOnlyList<ProductReviewId> ProductReviewIds => _productReviewIds.AsReadOnly();
    public DateTime CreatedOn { get; private set; }
    public DateTime ModifiedOn { get; private set; }

    public static Customer Create(string profileImageUrl, UserId userId, Address address)
    {
        return new Customer(CustomerId.CreateUnique(), profileImageUrl, userId, address, DateTime.UtcNow, DateTime.UtcNow);
    }
    private Customer(CustomerId customerId,  string profileImageUrl, UserId userId, Address address, DateTime createdOn, DateTime modifiedOn) : base(customerId)
    {
        ProfileImageUrl = profileImageUrl;
        UserId = userId;
        Address = address;
        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Customer() {}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

}

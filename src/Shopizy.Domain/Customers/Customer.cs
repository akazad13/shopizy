using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Customers.ValueObject;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Domain.Customers;

public sealed class Customer : AggregateRoot<CustomerId, Guid>
{
    public string profileImageUrl { get; private set; }
    public UserId UserId { get; }
    public DateTime CreatedDateTime { get; private set; }
    public DateTime UpdatedDateTime { get; private set; }
    // public IReadOnlyList< MyProperty { get; set; }
}

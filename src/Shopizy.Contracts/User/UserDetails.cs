using Shopizy.Contracts.Order;

namespace Shopizy.Contracts.User;

public sealed class UserDetails
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? Phone { get; set; }
    public Address? Address { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
}

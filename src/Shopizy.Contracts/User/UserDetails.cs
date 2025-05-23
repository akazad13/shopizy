using Shopizy.Contracts.Order;

namespace Shopizy.Contracts.User;

public record UserDetails(
    Guid Id,
    string? FirstName,
    string? LastName,
    string Email,
    string? ProfileImageUrl,
    string? Phone,
    Address? Address,
    int TotalOrders,
    int TotalReviewed,
    int TotalFavorites,
    int TotalReturns,
    DateTime CreatedOn,
    DateTime? ModifiedOn
);

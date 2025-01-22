namespace Shopizy.Contracts.User;

public record UpdateUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    UpdateAddressRequest Address
);

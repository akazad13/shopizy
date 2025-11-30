namespace Shopizy.Contracts.User;

/// <summary>
/// Represents a request to update user profile information.
/// </summary>
/// <param name="FirstName">The user's first name.</param>
/// <param name="LastName">The user's last name.</param>
/// <param name="Email">The user's email address.</param>
/// <param name="PhoneNumber">The user's phone number.</param>
/// <param name="Address">The user's address details.</param>
public record UpdateUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    UpdateAddressRequest Address
);

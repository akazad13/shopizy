namespace Shopizy.Contracts.Authentication;

/// <summary>
/// Represents the authentication response containing the token and user details.
/// </summary>
/// <param name="Id">The user's unique identifier.</param>
/// <param name="FirstName">The user's first name.</param>
/// <param name="LastName">The user's last name.</param>
/// <param name="Email">The user's email address.</param>
/// <param name="Role">The user's role.</param>
/// <param name="Token">The JWT access token.</param>
/// <param name="RefreshToken">The opaque refresh token. Use it against /auth/refresh to mint a new access + refresh token pair.</param>
/// <param name="RefreshTokenExpiresAt">UTC expiration time of the refresh token.</param>
public record AuthResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Role,
    string Token,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt);

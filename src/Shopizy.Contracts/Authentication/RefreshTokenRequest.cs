namespace Shopizy.Contracts.Authentication;

/// <summary>
/// Request to exchange a refresh token for a new access + refresh token pair.
/// </summary>
/// <param name="RefreshToken">The opaque refresh token returned from /auth/login.</param>
public record RefreshTokenRequest(string RefreshToken);

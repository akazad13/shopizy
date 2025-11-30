namespace Shopizy.Contracts.Authentication;

/// <summary>
/// Represents a user registration request.
/// </summary>
/// <param name="FirstName">The user's first name.</param>
/// <param name="LastName">The user's last name.</param>
/// <param name="Email">The user's email address.</param>
/// <param name="Password">The user's password.</param>
public record RegisterRequest(string FirstName, string LastName, string Email, string Password);

using ErrorOr;
using MediatR;

namespace Shopizy.Application.Auth.Commands.Register;

/// <summary>
/// Represents a command to register a new user account.
/// </summary>
/// <param name="FirstName">The user's first name.</param>
/// <param name="LastName">The user's last name.</param>
/// <param name="Email">The user's email address.</param>
/// <param name="Password">The user's password.</param>
public record RegisterCommand(string FirstName, string LastName, string Email, string Password)
    : IRequest<ErrorOr<Success>>;

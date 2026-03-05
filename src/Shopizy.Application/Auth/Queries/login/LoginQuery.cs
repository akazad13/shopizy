using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Application.Auth.Common;

namespace Shopizy.Application.Auth.Queries.login;

/// <summary>
/// Represents a query to authenticate a user and generate an access token.
/// </summary>
/// <param name="Email">The user's email address.</param>
/// <param name="Password">The user's password.</param>
public record LoginQuery(string Email, string Password) : IQuery<ErrorOr<AuthResult>>;

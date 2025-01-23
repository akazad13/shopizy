using ErrorOr;
using MediatR;
using Shopizy.Application.Auth.Common;

namespace Shopizy.Application.Auth.Queries.login;

public record LoginQuery(string Email, string Password) : IRequest<ErrorOr<AuthResult>>;

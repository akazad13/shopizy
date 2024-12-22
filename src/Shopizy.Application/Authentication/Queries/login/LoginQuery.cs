using ErrorOr;
using MediatR;
using Shopizy.Application.Authentication.Common;

namespace Shopizy.Application.Authentication.Queries.login;

public record LoginQuery(string Email, string Password) : IRequest<ErrorOr<AuthResult>>;

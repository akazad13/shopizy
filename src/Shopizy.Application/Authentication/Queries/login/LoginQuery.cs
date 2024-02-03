using ErrorOr;
using MediatR;
using Shopizy.Application.Authentication.Common;

namespace Shopizy.Application.Authentication.Queries.login;

public record LoginQuery(string Phone, string Password) : IRequest<ErrorOr<AuthResult>>;

using MediatR;
using Shopizy.Application.Authentication.Common;
using Shopizy.Application.Common.Wrappers;

namespace Shopizy.Application.Authentication.Queries.login;

public record LoginQuery(string Phone, string Password) : IRequest<IResult<AuthResult>>;

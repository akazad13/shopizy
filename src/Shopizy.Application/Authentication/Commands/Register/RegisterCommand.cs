using MediatR;
using Shopizy.Application.Authentication.Common;
using Shopizy.Application.Common.Wrappers;

namespace Shopizy.Application.Authentication.Commands.Register;

public record RegisterCommand(string FirstName, string LastName, string Phone, string Password)
    : IRequest<IResult<AuthResult>>;

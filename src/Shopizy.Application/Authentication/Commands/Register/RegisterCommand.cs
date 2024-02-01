using MediatR;
using Shopizy.Application.Authentication.Common;

namespace Shopizy.Application.Authentication.Commands.Register;

public record RegisterCommand(string FirstName, string LastName, string Email, string Password)
    : IRequest<AuthenticationResult>;

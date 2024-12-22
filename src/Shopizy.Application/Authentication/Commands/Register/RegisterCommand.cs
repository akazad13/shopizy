using ErrorOr;
using MediatR;

namespace Shopizy.Application.Authentication.Commands.Register;

public record RegisterCommand(string FirstName, string LastName, string Email, string Password)
    : IRequest<ErrorOr<Success>>;

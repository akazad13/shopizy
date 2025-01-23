using ErrorOr;
using MediatR;

namespace Shopizy.Application.Auth.Commands.Register;

public record RegisterCommand(string FirstName, string LastName, string Email, string Password)
    : IRequest<ErrorOr<Success>>;

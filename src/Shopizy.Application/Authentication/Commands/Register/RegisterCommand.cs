using ErrorOr;
using MediatR;

namespace Shopizy.Application.Authentication.Commands.Register;

public record RegisterCommand(string FirstName, string LastName, string Phone, string Password)
    : IRequest<ErrorOr<Success>>;

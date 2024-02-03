using MediatR;
using Shopizy.Application.Authentication.Common;
using Shopizy.Application.Common.Interfaces;

namespace Shopizy.Application.Authentication.Commands.Register;

public class RegisterCommandHandler(IJwtTokenGenerator _jwtTokenGenerator) : IRequestHandler<RegisterCommand, AuthResult>
{
    public async Task<AuthResult> Handle(
        RegisterCommand command,
        CancellationToken cancellationToken
    )
    {
        // Check if user already exists

        // Create user (generate unique ID)

        // Create JWT Token
        Guid userId = Guid.NewGuid();

        var roles = new List<string>();
        var permissions = new List<string>();

        var token = _jwtTokenGenerator.GenerateToken(userId, command.FirstName, command.LastName, roles, permissions);

        return new AuthResult(
            Guid.NewGuid(),
            command.FirstName,
            command.LastName,
            command.Phone,
            "token"
        );
    }
}

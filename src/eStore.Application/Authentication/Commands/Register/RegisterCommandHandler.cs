using eStore.Application.Authentication.Common;
using eStore.Application.Common.Interfaces;
using MediatR;

namespace eStore.Application.Authentication.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthenticationResult>
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RegisterCommandHandler(IJwtTokenGenerator jwtTokenGenerator)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthenticationResult> Handle(
        RegisterCommand command,
        CancellationToken cancellationToken
    )
    {
        // Check if user already exists

        // Create user (generate unique ID)

        // Create JWT Token
        Guid userId = Guid.NewGuid();

        var token = _jwtTokenGenerator.GenerateToken(userId, command.FirstName, command.LastName);

        return new AuthenticationResult(
            Guid.NewGuid(),
            command.FirstName,
            command.LastName,
            command.Email,
            "token"
        );
    }
}

using ErrorOr;
using MediatR;
using Shopizy.Application.Authentication.Common;
using Shopizy.Application.Common.Interfaces.Authentication;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.Errors;
using Shopizy.Domain.Users;

namespace Shopizy.Application.Authentication.Commands.Register;

public class RegisterCommandHandler(IUserRepository _userRepository, IJwtTokenGenerator _jwtTokenGenerator) : IRequestHandler<RegisterCommand, ErrorOr<AuthResult>>
{
    public async Task<ErrorOr<AuthResult>> Handle(
        RegisterCommand command,
        CancellationToken cancellationToken
    )
    {
        if((await _userRepository.GetUserByPhone(command.Phone)) is not null)
        {
            return Errors.User.DuplicatePhone;
        }

        var user = User.Create(command.FirstName,
            command.LastName,
            command.Phone,
            command.Password);

        await _userRepository.Add(user);

        var roles = new List<string>();
        var permissions = new List<string>();

        var token = _jwtTokenGenerator.GenerateToken(user.Id.Value, command.FirstName, command.LastName, roles, permissions);

        return new AuthResult(
            Guid.NewGuid(),
            command.FirstName,
            command.LastName,
            command.Phone,
            token
        );
    }
}

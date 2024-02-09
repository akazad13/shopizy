using ErrorOr;
using MediatR;
using Shopizy.Application.Authentication.Common;
using Shopizy.Application.Common.Interfaces.Authentication;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.Errors;
using Shopizy.Domain.Users;

namespace Shopizy.Application.Authentication.Commands.Register;

public class RegisterCommandHandler(IUserRepository _userRepository, IJwtTokenGenerator _jwtTokenGenerator, IPasswordManager _passwordManager) : IRequestHandler<RegisterCommand, ErrorOr<AuthResult>>
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

        var hashedPassword = _passwordManager.CreateHashString(command.Password);

        var user = User.Create(command.FirstName,
            command.LastName,
            command.Phone,
            hashedPassword);

        await _userRepository.Add(user);

        if(!await _userRepository.Commit(cancellationToken))
        {
            return Errors.User.UserNotAdded;
        }

        var roles = new List<string>();
        var permissions = new List<string>();

        var token = _jwtTokenGenerator.GenerateToken(user.Id, command.FirstName, command.LastName, roles, permissions);

        return new AuthResult(
            user.Id.Value,
            user.FirstName,
            user.LastName,
            user.Phone,
            token
        );
    }
}

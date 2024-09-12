using ErrorOr;
using MediatR;
using Shopizy.Application.Authentication.Common;
using Shopizy.Application.Common.Interfaces.Authentication;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Users;

namespace Shopizy.Application.Authentication.Commands.Register;

public class RegisterCommandHandler(
    IUserRepository userRepository,
    IJwtTokenGenerator jwtTokenGenerator,
    IPasswordManager passwordManager
) : IRequestHandler<RegisterCommand, ErrorOr<AuthResult>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly IPasswordManager _passwordManager = passwordManager;

    public async Task<ErrorOr<AuthResult>> Handle(
        RegisterCommand command,
        CancellationToken cancellationToken
    )
    {
        if ((await _userRepository.GetUserByPhone(command.Phone)) is not null)
        {
            return CustomErrors.User.DuplicatePhone;
        }

        string hashedPassword = _passwordManager.CreateHashString(command.Password);

        var user = User.Create(command.FirstName, command.LastName, command.Phone, hashedPassword);

        await _userRepository.AddAsync(user);

        if (await _userRepository.Commit(cancellationToken) <= 0)
        {
            return CustomErrors.User.UserNotCreated;
        }

        var roles = new List<string>();
        var permissions = new List<string>();

        string token = _jwtTokenGenerator.GenerateToken(
            user.Id,
            command.FirstName,
            command.LastName,
            command.Phone,
            roles,
            permissions
        );

        return new AuthResult(user.Id.Value, user.FirstName, user.LastName, user.Phone, token);
    }
}

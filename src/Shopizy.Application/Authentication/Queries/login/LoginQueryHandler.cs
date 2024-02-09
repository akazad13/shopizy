using ErrorOr;
using MediatR;
using Shopizy.Application.Authentication.Common;
using Shopizy.Application.Common.Interfaces.Authentication;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.Errors;

namespace Shopizy.Application.Authentication.Queries.login;

public class LoginQueryHandler(IUserRepository _userRepository, IJwtTokenGenerator _jwtTokenGenerator, IPasswordManager _passwordManager) : IRequestHandler<LoginQuery, ErrorOr<AuthResult>>
{
    public async Task<ErrorOr<AuthResult>> Handle(LoginQuery query, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByPhone(query.Phone);
        if(user is null)
            return Errors.User.UserNotFound;
        if(!_passwordManager.Verify(query.Password, user.Password))
            return Errors.Authentication.InvalidCredentials;

        var roles = new List<string>();
        var permissions = new List<string>();

        var token = _jwtTokenGenerator.GenerateToken(user.Id, user.FirstName, user.LastName, roles, permissions);

        return new AuthResult(
            user.Id.Value,
            user.FirstName,
            user.LastName,
            user.Phone,
            token
        );
    }
}
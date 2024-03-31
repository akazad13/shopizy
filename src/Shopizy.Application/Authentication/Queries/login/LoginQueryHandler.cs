using ErrorOr;
using MediatR;
using Shopizy.Application.Authentication.Common;
using Shopizy.Application.Common.Interfaces.Authentication;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Roles;
using Shopizy.Domain.Common.CustomErrors;

namespace Shopizy.Application.Authentication.Queries.login;

public class LoginQueryHandler(IUserRepository _userRepository, IJwtTokenGenerator _jwtTokenGenerator, IPasswordManager _passwordManager) : IRequestHandler<LoginQuery, ErrorOr<AuthResult>>
{
    public async Task<ErrorOr<AuthResult>> Handle(LoginQuery query, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByPhone(query.Phone);
        if (user is null)
            return CustomErrors.User.UserNotFound;
        if (!_passwordManager.Verify(query.Password, user.Password!))
            return CustomErrors.Authentication.InvalidCredentials;

        var roles = new List<string>() { Role.Admin };
        var permissions = new List<string>(){
            Permission.Category.Create,
            Permission.Category.Get,
            Permission.Category.Modify,
            Permission.Category.Delete,
            Permission.Product.Create,
            Permission.Product.Get,
            Permission.Product.Modify,
            Permission.Product.Delete,
            Permission.Cart.Create,
            Permission.Cart.Get,
            Permission.Cart.Modify,
            Permission.Cart.Delete
        };

        var token = _jwtTokenGenerator.GenerateToken(user.Id, user.FirstName, user.LastName, user.Phone, roles, permissions);

        return new AuthResult(
            user.Id.Value,
            user.FirstName,
            user.LastName,
            user.Phone,
            token
        );
    }
}
using ErrorOr;
using MediatR;
using Shopizy.Application.Authentication.Common;
using Shopizy.Application.Common.Interfaces.Authentication;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Security.Permissions;
using Shopizy.Application.Common.Security.Roles;
using Shopizy.Domain.Common.CustomErrors;

namespace Shopizy.Application.Authentication.Queries.login;

public class LoginQueryHandler(
    IUserRepository userRepository,
    IJwtTokenGenerator jwtTokenGenerator,
    IPasswordManager passwordManager
) : IRequestHandler<LoginQuery, ErrorOr<AuthResult>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly IPasswordManager _passwordManager = passwordManager;

    public async Task<ErrorOr<AuthResult>> Handle(
        LoginQuery query,
        CancellationToken cancellationToken
    )
    {
        var user = await _userRepository.GetUserByPhone(query.Phone);
        if (user is null)
        {
            return CustomErrors.User.UserNotFoundWhileLogin;
        }

        if (!_passwordManager.Verify(query.Password, user.Password!))
        {
            return CustomErrors.Authentication.InvalidCredentials;
        }

        var roles = new List<string>() { Role.Admin };
        var permissions = new List<string>()
        {
            Permissions.Category.Create,
            Permissions.Category.Get,
            Permissions.Category.Modify,
            Permissions.Category.Delete,
            Permissions.Product.Create,
            Permissions.Product.Get,
            Permissions.Product.Modify,
            Permissions.Product.Delete,
            Permissions.Cart.Create,
            Permissions.Cart.Get,
            Permissions.Cart.Modify,
            Permissions.Cart.Delete,
            Permissions.Order.Create,
            Permissions.Order.Get,
            Permissions.Order.Modify,
            Permissions.Order.Delete,
            Permissions.User.Modify,
            Permissions.User.Get,
            Permissions.User.Create,
            Permissions.User.Delete,
        };

        var token = _jwtTokenGenerator.GenerateToken(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Phone,
            roles,
            permissions
        );

        return new AuthResult(user.Id.Value, user.FirstName, user.LastName, user.Phone, token);
    }
}

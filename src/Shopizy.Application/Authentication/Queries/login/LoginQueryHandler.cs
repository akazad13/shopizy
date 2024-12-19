using ErrorOr;
using MediatR;
using Shopizy.Application.Authentication.Common;
using Shopizy.Application.Common.Interfaces.Authentication;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;

namespace Shopizy.Application.Authentication.Queries.login;

public class LoginQueryHandler(
    IUserRepository userRepository,
    IPermissionRepository permissionRepository,
    IJwtTokenGenerator jwtTokenGenerator,
    IPasswordManager passwordManager
) : IRequestHandler<LoginQuery, ErrorOr<AuthResult>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPermissionRepository _permissionRepository = permissionRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly IPasswordManager _passwordManager = passwordManager;

    public async Task<ErrorOr<AuthResult>> Handle(
        LoginQuery query,
        CancellationToken cancellationToken
    )
    {
        var user = await _userRepository.GetUserByPhoneAsync(query.Phone);
        if (user is null)
        {
            return CustomErrors.User.UserNotFoundWhileLogin;
        }

        if (!_passwordManager.Verify(query.Password, user.Password!))
        {
            return CustomErrors.Authentication.InvalidCredentials;
        }

        var allPermissions = await _permissionRepository.GetAsync();

        var assignedPermissions = allPermissions
            .Where(permission => user.PermissionIds.Contains(permission.Id))
            .Select(p => p.Name);

        var roles = new List<string>() { };

        var token = _jwtTokenGenerator.GenerateToken(user.Id, roles, assignedPermissions);

        return new AuthResult(user.Id.Value, user.FirstName, user.LastName, user.Phone, token);
    }
}

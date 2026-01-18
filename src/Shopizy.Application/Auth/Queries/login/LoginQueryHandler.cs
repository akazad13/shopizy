using ErrorOr;
using MediatR;
using Shopizy.Application.Auth.Common;
using Shopizy.Application.Common.Interfaces.Authentication;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Common.CustomErrors;

namespace Shopizy.Application.Auth.Queries.login;

/// <summary>
/// Handles the <see cref="LoginQuery"/> to authenticate users and generate JWT tokens.
/// </summary>
public class LoginQueryHandler(
    IUserRepository userRepository,
    IPermissionRepository permissionRepository,
    IJwtTokenGenerator jwtTokenGenerator,
    IPasswordManager passwordManager,
    ICartRepository cartRepository
) : IRequestHandler<LoginQuery, ErrorOr<AuthResult>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPermissionRepository _permissionRepository = permissionRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly IPasswordManager _passwordManager = passwordManager;
    private readonly ICartRepository _cartRepository = cartRepository;

    /// <summary>
    /// Handles the login process including credential validation, permission loading, and token generation.
    /// </summary>
    /// <param name="query">The login query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An authentication result containing user information and JWT token, or an error.</returns>
    public async Task<ErrorOr<AuthResult>> Handle(
        LoginQuery query,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await _userRepository.GetUserByEmailAsync(query.Email);
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

        if (user.Id.Value == Guid.Parse("E68D8E76-72A1-42ED-91D0-0ED0296D662E"))
        {
            roles.Add("Admin");
        }

        var token = _jwtTokenGenerator.GenerateToken(user.Id, roles, assignedPermissions);

        var cart = await _cartRepository.GetCartByUserIdAsync(user.Id);

        if (cart is null)
        {
            cart = Cart.Create(user.Id);
            await _cartRepository.AddAsync(cart);
            await _cartRepository.CommitAsync(cancellationToken);
        }

        return new AuthResult(user.Id.Value, user.FirstName, user.LastName, user.Email, token);
    }
}

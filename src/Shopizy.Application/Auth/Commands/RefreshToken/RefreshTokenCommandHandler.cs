using ErrorOr;
using Shopizy.Application.Auth.Common;
using Shopizy.Application.Common.Interfaces.Authentication;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler(
    IUserRepository userRepository,
    IPermissionRepository permissionRepository,
    IJwtTokenGenerator jwtTokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator,
    IRefreshTokenStore refreshTokenStore
) : ICommandHandler<RefreshTokenCommand, ErrorOr<AuthResult>>
{
    public async Task<ErrorOr<AuthResult>> Handle(
        RefreshTokenCommand command,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(command);

        var userId = await refreshTokenStore.ConsumeAsync(command.RefreshToken, cancellationToken);
        if (userId is null)
        {
            return (Error)CustomErrors.Authentication.InvalidCredentials;
        }

        var user = await userRepository.GetUserByIdAsync(userId);
        if (user is null)
        {
            return (Error)CustomErrors.User.UserNotFoundWhileLogin;
        }

        var allPermissions = await permissionRepository.GetAsync();
        var assignedPermissions = allPermissions
            .Where(permission => user.PermissionIds.Any(up => up.Value == permission.Id.Value))
            .Select(p => p.Name)
            .ToList();

        var accessToken = jwtTokenGenerator.GenerateToken(
            user.Id,
            user.Role.ToString(),
            assignedPermissions
        );

        var newRefresh = refreshTokenGenerator.Generate();
        var lifetime = refreshTokenGenerator.Lifetime;
        await refreshTokenStore.StoreAsync(newRefresh, user.Id, lifetime, cancellationToken);
        var expiresAt = DateTime.UtcNow.Add(lifetime);

        return new AuthResult(
            user.Id.Value,
            user.FirstName,
            user.LastName,
            user.Email,
            user.Role.ToString(),
            accessToken,
            newRefresh,
            expiresAt
        );
    }
}

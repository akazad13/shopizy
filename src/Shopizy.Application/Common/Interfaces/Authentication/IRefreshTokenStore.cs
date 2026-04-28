using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Authentication;

public interface IRefreshTokenStore
{
    Task StoreAsync(
        string token,
        UserId userId,
        TimeSpan ttl,
        CancellationToken cancellationToken = default
    );

    Task<UserId?> ConsumeAsync(string token, CancellationToken cancellationToken = default);

    Task RevokeAsync(string token, CancellationToken cancellationToken = default);

    Task RevokeAllForUserAsync(UserId userId, CancellationToken cancellationToken = default);
}

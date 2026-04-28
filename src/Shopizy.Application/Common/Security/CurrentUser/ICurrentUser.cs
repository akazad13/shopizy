namespace Shopizy.Application.Common.Security.CurrentUser;

public interface ICurrentUser
{
    Guid GetCurrentUserId();

    /// <summary>
    /// Attempts to retrieve the current user ID without throwing.
    /// Returns false when there is no active HTTP context or the "id" claim is absent
    /// (e.g. anonymous or unauthenticated requests).
    /// </summary>
    bool TryGetCurrentUserId(out Guid userId);
}

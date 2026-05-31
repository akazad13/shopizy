using System.Security.Claims;
using Shopizy.Api.Endpoints;

namespace Shopizy.Api.Common.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Returns true if the authenticated principal owns the resource keyed by the given user id.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="userId"></param>
    public static bool IsAuthorized(this ClaimsPrincipal user, Guid userId)
    {
        ArgumentNullException.ThrowIfNull(user);

        var currentUserIdClaim =
            user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user.FindFirst("id")?.Value;

        return (currentUserIdClaim != null) && currentUserIdClaim == userId.ToString();
    }

    /// <summary>
    /// Returns a <c>403 Forbidden</c> result if the principal does not own the resource;
    /// returns null when authorized so the caller can proceed.
    /// Usage:
    /// <code>
    /// if (user.AuthorizeOwner(userId, "this order") is { } forbidden) return forbidden;
    /// </code>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="userId"></param>
    /// <param name="resourceName"></param>
    public static IResult? AuthorizeOwner(
        this ClaimsPrincipal user,
        Guid userId,
        string resourceName
    ) =>
        user.IsAuthorized(userId)
            ? null
            : CustomResults.Problem([
                ErrorOr.Error.Forbidden(
                    description: $"You are not authorized to access {resourceName}."
                ),
            ]);
}

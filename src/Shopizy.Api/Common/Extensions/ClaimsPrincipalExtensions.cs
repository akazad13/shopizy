using System.Security.Claims;

namespace Shopizy.Api.Common.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static bool IsAuthorized(this ClaimsPrincipal user, Guid userId)
    {
        ArgumentNullException.ThrowIfNull(user);

        var currentUserIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? user.FindFirst("id")?.Value;

        return (currentUserIdClaim != null) && currentUserIdClaim == userId.ToString();
    }
}

using System.Security.Claims;

namespace Shopizy.Api.Common.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static bool IsAuthorized(this ClaimsPrincipal user, Guid userId)
    {
        var currentUserIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? user.FindFirst("id")?.Value;
            
        if (currentUserIdClaim == null) return false;
        
        return currentUserIdClaim == userId.ToString() || user.IsInRole("Admin");
    }
}

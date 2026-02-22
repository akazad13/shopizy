using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Http;

namespace Shopizy.Application.Common.Security.CurrentUser;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public Guid GetCurrentUserId()
    {
        Guard.Against.Null(_httpContextAccessor.HttpContext);

        var idClaim = _httpContextAccessor.HttpContext!.User.FindFirst("id")?.Value 
            ?? _httpContextAccessor.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
        if (idClaim == null)
        {
            throw new InvalidOperationException("User ID claim not found in current context.");
        }

        return Guid.Parse(idClaim);
    }
}

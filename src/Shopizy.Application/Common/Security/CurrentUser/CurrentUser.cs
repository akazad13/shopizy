using Microsoft.AspNetCore.Http;

namespace Shopizy.Application.Common.Security.CurrentUser;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public Guid GetCurrentUserId()
    {
        ArgumentNullException.ThrowIfNull(_httpContextAccessor.HttpContext);

        var idClaim = _httpContextAccessor.HttpContext!.User.FindFirst("id")?.Value
            ?? _httpContextAccessor.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (idClaim == null)
        {
            throw new InvalidOperationException("User ID claim not found in current context.");
        }

        return Guid.Parse(idClaim);
    }

    public bool TryGetCurrentUserId(out Guid userId)
    {
        userId = Guid.Empty;

        if (_httpContextAccessor.HttpContext is null)
        {
            return false;
        }

        var idClaim = _httpContextAccessor.HttpContext.User.FindFirst("id")?.Value
            ?? _httpContextAccessor.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (idClaim is null || !Guid.TryParse(idClaim, out userId))
        {
            return false;
        }

        return true;
    }
}


using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Http;

namespace Shopizy.Application.Common.Security.CurrentUser;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public Guid GetCurrentUserId()
    {
        Guard.Against.Null(_httpContextAccessor.HttpContext);

        var id = _httpContextAccessor
            .HttpContext!.User.Claims.Single(claim => claim.Type == "id")
            .Value;
        return Guid.Parse(id);
    }
}

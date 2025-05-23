using System.Security.Claims;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Http;

namespace Shopizy.Infrastructure.Security.CurrentUserProvider;

public class CurrentUserProvider(IHttpContextAccessor httpContextAccessor) : ICurrentUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public CurrentUser? GetCurrentUser()
    {
        Guard.Against.Null(_httpContextAccessor.HttpContext);

        if (!_httpContextAccessor.HttpContext!.User.Claims.Any())
        {
            return null;
        }

        var id = Guid.Parse(GetSingleClaimValue("id"));
        List<string> permissions = GetClaimValues("permissions");
        List<string> roles = GetClaimValues(ClaimTypes.Role);

        return new CurrentUser(id, permissions, roles);
    }

    private List<string> GetClaimValues(string claimType) =>
        _httpContextAccessor
            .HttpContext!.User.Claims.Where(claim => claim.Type == claimType)
            .Select(claim => claim.Value)
            .ToList();

    private string GetSingleClaimValue(string claimType) =>
        _httpContextAccessor
            .HttpContext!.User.Claims.Single(claim => claim.Type == claimType)
            .Value;
}

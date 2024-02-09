using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Throw;

namespace Shopizy.Infrastructure.Security.CurrentUserProvider;

public class CurrentUserProvider(IHttpContextAccessor _httpContextAccessor) : ICurrentUserProvider
{
    public CurrentUser GetCurrentUser()
    {
        _httpContextAccessor.HttpContext.ThrowIfNull();

        var id = Guid.Parse(GetSingleClaimValue(JwtRegisteredClaimNames.NameId));
        var permissions = GetClaimValues("permissions");
        var roles = GetClaimValues(ClaimTypes.Role);
        var firstName = GetSingleClaimValue(JwtRegisteredClaimNames.Name);
        var lastName = GetSingleClaimValue(JwtRegisteredClaimNames.FamilyName);
        var phone = GetSingleClaimValue(ClaimTypes.MobilePhone);

        return new CurrentUser(id, firstName, lastName, phone, permissions, roles);
    }


    private List<string> GetClaimValues(string claimType) =>
        _httpContextAccessor.HttpContext!.User.Claims
            .Where(claim => claim.Type == claimType)
            .Select(claim => claim.Value)
            .ToList();
    private string GetSingleClaimValue(string claimType) => 
        _httpContextAccessor.HttpContext!.User.Claims
            .Single(claim => claim.Type == claimType)
            .Value;
}
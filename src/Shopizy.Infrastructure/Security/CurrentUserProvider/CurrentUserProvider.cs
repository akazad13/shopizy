using System.Security.Claims;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Shopizy.Infrastructure.Security.CurrentUserProvider;

public class CurrentUserProvider(
    IHttpContextAccessor httpContextAccessor,
    ILogger<CurrentUserProvider> logger) : ICurrentUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ILogger<CurrentUserProvider> _logger = logger;

    public CurrentUser? GetCurrentUser()
    {
        try
        {
            Guard.Against.Null(_httpContextAccessor.HttpContext);

            if (_httpContextAccessor.HttpContext?.User?.Claims == null || 
                !_httpContextAccessor.HttpContext.User.Claims.Any())
            {
                _logger.LogDebug("No claims found in HttpContext");
                return null;
            }

            var idClaim = GetSingleClaimValue("id");
            if (idClaim == null || !Guid.TryParse(idClaim, out var id))
            {
                _logger.LogWarning("Invalid or missing 'id' claim");
                return null;
            }

            List<string> permissions = GetClaimValues("permissions");
            List<string> roles = GetClaimValues(ClaimTypes.Role);

            return new CurrentUser(id, permissions, roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting current user from HttpContext");
            return null;
        }
    }

    private List<string> GetClaimValues(string claimType) =>
        _httpContextAccessor
            .HttpContext?.User?.Claims
            .Where(claim => claim.Type == claimType)
            .Select(claim => claim.Value)
            .ToList() ?? [];

    private string? GetSingleClaimValue(string claimType) =>
        _httpContextAccessor
            .HttpContext?.User?.Claims
            .FirstOrDefault(claim => claim.Type == claimType)
            ?.Value;
}

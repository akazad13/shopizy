using System.Security.Claims;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Shopizy.Infrastructure.Security.CurrentUserProvider;

public partial class CurrentUserProvider(
    IHttpContextAccessor httpContextAccessor,
    ILogger<CurrentUserProvider> logger) : ICurrentUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ILogger<CurrentUserProvider> _logger = logger;

    [LoggerMessage(Level = LogLevel.Debug, Message = "No claims found in HttpContext")]
    static partial void LogNoClaimsFound(ILogger logger);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Invalid or missing 'id' claim")]
    static partial void LogInvalidIdClaim(ILogger logger);

    [LoggerMessage(Level = LogLevel.Error, Message = "Error extracting current user from HttpContext")]
    static partial void LogUserExtractionError(ILogger logger, Exception ex);

    public CurrentUser? GetCurrentUser()
    {
        try
        {
            Guard.Against.Null(_httpContextAccessor.HttpContext);

            if (_httpContextAccessor.HttpContext?.User?.Claims == null || 
                !_httpContextAccessor.HttpContext.User.Claims.Any())
            {
                LogNoClaimsFound(_logger);
                return null;
            }

            var idClaim = GetSingleClaimValue("id");
            if (idClaim == null || !Guid.TryParse(idClaim, out var id))
            {
                LogInvalidIdClaim(_logger);
                return null;
            }

            List<string> permissions = GetClaimValues("permissions");
            List<string> roles = GetClaimValues(ClaimTypes.Role);

            return new CurrentUser(id, permissions, roles);
        }
        catch (Exception ex)
        {
            LogUserExtractionError(_logger, ex);
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

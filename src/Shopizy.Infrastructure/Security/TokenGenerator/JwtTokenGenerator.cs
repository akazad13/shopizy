using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shopizy.Application.Common.Interfaces.Authentication;
using Shopizy.Domain.Users.ValueObjects;
using System.Diagnostics.CodeAnalysis;

namespace Shopizy.Infrastructure.Security.TokenGenerator;

/// <summary>
/// Generates JWT tokens for authentication.
/// </summary>
[ExcludeFromCodeCoverage]
public class JwtTokenGenerator(IOptions<JwtSettings> jwtOptoins) : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings = jwtOptoins.Value;

    /// <summary>
    /// Generates a JWT token for a user with specified role and permissions.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <param name="role">The user's role.</param>
    /// <param name="Permissions">The list of permissions assigned to the user.</param>
    /// <returns>A JWT token string.</returns>
    public string GenerateToken(UserId userId, string role, IEnumerable<string> Permissions)
    {
        ArgumentNullException.ThrowIfNull(Permissions);
        ArgumentNullException.ThrowIfNull(userId);

        var claims = new List<Claim> 
        { 
            new("id", userId.Value.ToString()),
            new(ClaimTypes.Role, role)
        };

        foreach (string permission in Permissions)
        {
            claims.Add(new("permissions", permission));
        }

        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
            SecurityAlgorithms.HmacSha256
        );

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            IssuedAt = DateTime.UtcNow,
            NotBefore = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpirationMinutes),
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = creds,
        };

        var jwtTokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwtToken = jwtTokenHandler.CreateJwtSecurityToken(tokenDescriptor);
        string token = jwtTokenHandler.WriteToken(jwtToken);
        return token;
    }
}

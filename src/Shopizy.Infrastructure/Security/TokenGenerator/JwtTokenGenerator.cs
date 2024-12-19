using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shopizy.Application.Common.Interfaces.Authentication;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Infrastructure.Security.TokenGenerator;

public class JwtTokenGenerator(IOptions<JwtSettings> jwtOptoins) : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings = jwtOptoins.Value;

    public string GenerateToken(UserId userId, IList<string> roles, IEnumerable<string> Permissions)
    {
        Guard.Against.Null(roles);
        Guard.Against.Null(Permissions);
        Guard.Against.Null(userId);

        var claims = new List<Claim> { new("id", userId.Value.ToString()) };

        foreach (string role in roles)
        {
            claims.Add(new(ClaimTypes.Role, role));
        }

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

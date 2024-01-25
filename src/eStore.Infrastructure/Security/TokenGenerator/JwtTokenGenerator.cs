using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using eStore.Application.Common.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace eStore.Infrastructure.Authentication;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    public string GenerateToken(Guid userId, string firstName, string LastName)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.GivenName, firstName),
            new Claim(ClaimTypes.Surname, LastName)
        };

        // user roles

        // signing credentials

        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes("lsdfjsdlf")),
            SecurityAlgorithms.HmacSha256
        );

        var validIssuer = "";
        var validAudience = "";
        var expiryDuration = 30;

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = validIssuer,
            Audience = validAudience,
            IssuedAt = DateTime.UtcNow,
            NotBefore = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(expiryDuration),
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = creds
        };

        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = jwtTokenHandler.CreateJwtSecurityToken(tokenDescriptor);
        var token = jwtTokenHandler.WriteToken(jwtToken);
        return token;
    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Domain.Users.Enums;
using Shopizy.Infrastructure.Security.TokenGenerator;
using Xunit;

namespace Shopizy.Infrastructure.UnitTests.Security.TokenGenerator;

public class JwtTokenGeneratorTests
{
    private readonly JwtTokenGenerator _jwtTokenGenerator;

    public JwtTokenGeneratorTests()
    {
        var _jwtSettings = new JwtSettings
        {
            Secret = "a very secret value that will be used to generate the JWT token",
            Issuer = "issuer",
            Audience = "audience",
            TokenExpirationMinutes = 10,
        };

        _jwtTokenGenerator = new JwtTokenGenerator(Options.Create(_jwtSettings));
    }

    [Fact]
    public void ShouldCreateTokenWithCorrectClaimsWhenCalled()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        var role = UserRole.Customer.ToString();
        var permissions = new List<string> { "CanCreateProduct", "CanEditProduct" };

        // Act
        string token = _jwtTokenGenerator.GenerateToken(userId, role, permissions);

        // Assert
        var jwtToken = new JwtSecurityToken(token);

        // Claims: id (1), role (1), permissions (2), iat (1), nbf (1), exp (1), iss (1), aud (1) = 9 total
        Assert.Equal(9, jwtToken.Claims.Count());

        Assert.Contains(jwtToken.Claims, c =>
            c.Type == "id" && c.Value == userId.Value.ToString());

        Assert.Contains(jwtToken.Claims, c =>
            c.Type == ClaimTypes.Role && c.Value == role);

        Assert.Contains(jwtToken.Claims, c =>
            c.Type == "permissions" && c.Value == "CanCreateProduct");
        Assert.Contains(jwtToken.Claims, c =>
            c.Type == "permissions" && c.Value == "CanEditProduct");
    }
}

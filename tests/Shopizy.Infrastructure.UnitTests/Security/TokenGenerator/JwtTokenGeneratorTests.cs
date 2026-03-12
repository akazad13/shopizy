using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Shopizy.Domain.Users.ValueObjects;
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
        var permissions = new List<string> { "CanCreateProduct", "CanEditProduct" };

        // Act
        string token = _jwtTokenGenerator.GenerateToken(userId, permissions);

        // Assert
        var jwtToken = new JwtSecurityToken(token);

        Assert.Equal(8, jwtToken.Claims.Count());

        Assert.Contains(jwtToken.Claims, c =>
            c.Type == "id" && c.Value == userId.Value.ToString());

        Assert.Contains(jwtToken.Claims, c =>
            c.Type == "permissions" && c.Value == "CanCreateProduct");
        Assert.Contains(jwtToken.Claims, c =>
            c.Type == "permissions" && c.Value == "CanEditProduct");
    }
}

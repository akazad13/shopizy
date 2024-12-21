using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Shopizy.Application.Common.Interfaces.Authentication;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Infrastructure.Security.TokenGenerator;
using Xunit;

namespace Shopizy.Infrastructure.UnitTests.Security.TokenGenerator;

public class JwtTokenGeneratorTests
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

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
        var roles = new List<string> { "Admin", "Moderator" };
        var permissions = new List<string> { "CanCreateProduct", "CanEditProduct" };

        // Act
        string token = _jwtTokenGenerator.GenerateToken(userId, roles, permissions);

        // Assert
        var jwtToken = new JwtSecurityToken(token);

        jwtToken.Claims.Should().HaveCount(10);
        jwtToken.Claims.Should().Contain(c => c.Type == "id" && c.Value == userId.Value.ToString());

        jwtToken.Claims.Should().Contain(c => c.Type == "role" && c.Value == "Admin");
        jwtToken.Claims.Should().Contain(c => c.Type == "role" && c.Value == "Moderator");

        jwtToken
            .Claims.Should()
            .Contain(c => c.Type == "permissions" && c.Value == "CanCreateProduct");
        jwtToken
            .Claims.Should()
            .Contain(c => c.Type == "permissions" && c.Value == "CanEditProduct");
    }
}

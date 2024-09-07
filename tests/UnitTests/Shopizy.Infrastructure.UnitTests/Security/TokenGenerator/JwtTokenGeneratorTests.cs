using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
    public void GenerateToken_WhenCalled_CreateTokenWithCorrectClaims()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        string firstName = "John";
        string lastName = "Doe";
        string phone = "123456789";
        var roles = new List<string> { "Admin", "Moderator" };
        var permissions = new List<string> { "CanCreateProduct", "CanEditProduct" };

        // Act
        string token = _jwtTokenGenerator.GenerateToken(
            userId,
            firstName,
            lastName,
            phone,
            roles,
            permissions
        );

        // Assert
        var jwtToken = new JwtSecurityToken(token);

        _ = jwtToken.Claims.Should().HaveCount(13);
        _ = jwtToken
            .Claims.Should()
            .Contain(c => c.Type == "id" && c.Value == userId.Value.ToString());
        _ = jwtToken
            .Claims.Should()
            .Contain(c => c.Type == JwtRegisteredClaimNames.Name && c.Value == firstName);
        _ = jwtToken
            .Claims.Should()
            .Contain(c => c.Type == JwtRegisteredClaimNames.FamilyName && c.Value == lastName);
        _ = jwtToken
            .Claims.Should()
            .Contain(c => c.Type == ClaimTypes.MobilePhone && c.Value == phone);
        _ = jwtToken.Claims.Should().Contain(c => c.Type == "role" && c.Value == "Admin");
        _ = jwtToken.Claims.Should().Contain(c => c.Type == "role" && c.Value == "Moderator");

        _ = jwtToken
            .Claims.Should()
            .Contain(c => c.Type == "permissions" && c.Value == "CanCreateProduct");
        _ = jwtToken
            .Claims.Should()
            .Contain(c => c.Type == "permissions" && c.Value == "CanEditProduct");
    }
}

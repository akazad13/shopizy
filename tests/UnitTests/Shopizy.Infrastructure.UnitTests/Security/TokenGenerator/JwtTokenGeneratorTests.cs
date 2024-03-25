using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Shopizy.Application.Common.Interfaces.Authentication;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Infrastructure.Authentication;
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
            // Secret = "asdksdhcaskd33khoadfao934nasldasldjasldhasodhasodasldjasldjasld",
            Secret = "a very secret value that will be used to generate the JWT token",
            Issuer = "issuer",
            Audience = "audience",
            TokenExpirationMinutes = 10
        };

        _jwtTokenGenerator = new JwtTokenGenerator(Options.Create(_jwtSettings));
    }

    [Fact]
    public void GenerateToken_WhenCalled_ShouldCreateTokenWithCorrectClaims()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        var firstName = "John";
        var lastName = "Doe";
        var phone = "123456789";
        var roles = new List<string> { "Admin", "Moderator" };
        var permissions = new List<string> { "CanCreateProduct", "CanEditProduct" };

        // Act
        var token = _jwtTokenGenerator.GenerateToken(
            userId,
            firstName,
            lastName,
            phone,
            roles,
            permissions
        );

        // Assert
        var jwtToken = new JwtSecurityToken(token);

        jwtToken.Claims.Should().HaveCount(13);
        jwtToken.Claims.Should().Contain(c => c.Type == "id" && c.Value == userId.Value.ToString());
        jwtToken.Claims
            .Should()
            .Contain(c => c.Type == JwtRegisteredClaimNames.Name && c.Value == firstName);
        jwtToken.Claims
            .Should()
            .Contain(c => c.Type == JwtRegisteredClaimNames.FamilyName && c.Value == lastName);
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.MobilePhone && c.Value == phone);
        jwtToken.Claims.Should().Contain(c => c.Type == "role" && c.Value == "Admin");
        jwtToken.Claims.Should().Contain(c => c.Type == "role" && c.Value == "Moderator");

        jwtToken.Claims
            .Should()
            .Contain(c => c.Type == "permissions" && c.Value == "CanCreateProduct");
        jwtToken.Claims
            .Should()
            .Contain(c => c.Type == "permissions" && c.Value == "CanEditProduct");
    }
}

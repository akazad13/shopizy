using Microsoft.Extensions.Options;
using Shopizy.Infrastructure.Security.RefreshTokens;
using Shouldly;

namespace Shopizy.Infrastructure.UnitTests.Security.TokenGenerator;

public class RefreshTokenGeneratorTests
{
    [Fact]
    public void Generate_ShouldReturnNonEmptyUrlSafeString()
    {
        // Arrange
        var settings = Options.Create(new RefreshTokenSettings { ExpirationDays = 7 });
        var generator = new RefreshTokenGenerator(settings);

        // Act
        var token1 = generator.Generate();
        var token2 = generator.Generate();

        // Assert
        token1.ShouldNotBeNullOrWhiteSpace();
        token2.ShouldNotBeNullOrWhiteSpace();
        token1.ShouldNotBe(token2);
        token1.ShouldNotContain("+");
        token1.ShouldNotContain("/");
        token1.EndsWith("=").ShouldBeFalse();
        generator.Lifetime.TotalDays.ShouldBe(7);
    }
}

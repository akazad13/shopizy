using Shopizy.Domain.GiftCards;
using Shopizy.Domain.GiftCards.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shouldly;
using Xunit;

namespace Shopizy.Domain.UnitTests.GiftCards;

public class GiftCardTests
{
    [Fact]
    public void Create_ShouldInitializeGiftCard()
    {
        // Arrange & Act
        var giftCard = GiftCard.Create("GC100", 100m, DateTime.UtcNow.AddDays(30));

        // Assert
        giftCard.ShouldNotBeNull();
        giftCard.Code.ShouldBe("GC100");
        giftCard.InitialBalance.ShouldBe(100m);
        giftCard.RemainingBalance.ShouldBe(100m);
        giftCard.IsActive.ShouldBeTrue();
        giftCard.RedeemedByUserId.ShouldBeNull();
    }

    [Fact]
    public void Redeem_WhenActiveAndValid_ShouldSucceed()
    {
        // Arrange
        var giftCard = GiftCard.Create("GC100", 100m, DateTime.UtcNow.AddDays(30));
        var userId = UserId.CreateUnique();

        // Act
        var result = giftCard.Redeem(userId);

        // Assert
        result.IsError.ShouldBeFalse();
        giftCard.IsActive.ShouldBeFalse();
        giftCard.RedeemedByUserId.ShouldBe(userId);
        giftCard.RedeemedOn.ShouldNotBeNull();
    }

    [Fact]
    public void Redeem_WhenInactive_ShouldReturnError()
    {
        // Arrange
        var giftCard = GiftCard.Create("GC100", 100m, DateTime.UtcNow.AddDays(30));
        giftCard.Deactivate();

        // Act
        var result = giftCard.Redeem(UserId.CreateUnique());

        // Assert
        result.IsError.ShouldBeTrue();
    }

    [Fact]
    public void Redeem_WhenAlreadyRedeemed_ShouldReturnError()
    {
        // Arrange
        var giftCard = GiftCard.Create("GC100", 100m, DateTime.UtcNow.AddDays(30));
        var userId = UserId.CreateUnique();
        giftCard.Redeem(userId);

        // Act
        var result = giftCard.Redeem(userId);

        // Assert
        result.IsError.ShouldBeTrue();
    }

    [Fact]
    public void Redeem_WhenExpired_ShouldReturnError()
    {
        // Arrange
        var giftCard = GiftCard.Create("GC100", 100m, DateTime.UtcNow.AddDays(-1));

        // Act
        var result = giftCard.Redeem(UserId.CreateUnique());

        // Assert
        result.IsError.ShouldBeTrue();
    }

    [Fact]
    public void GiftCardId_CreateUnique_And_Create_ShouldInitialize()
    {
        var id1 = GiftCardId.CreateUnique();
        var rawGuid = Guid.NewGuid();
        var id2 = GiftCardId.Create(rawGuid);

        id1.Value.ShouldNotBe(Guid.Empty);
        id2.Value.ShouldBe(rawGuid);
    }
}

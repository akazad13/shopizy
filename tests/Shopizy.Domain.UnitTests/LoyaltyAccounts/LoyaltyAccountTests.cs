using Shopizy.Domain.LoyaltyAccounts;
using Shopizy.Domain.LoyaltyAccounts.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shouldly;
using Xunit;

namespace Shopizy.Domain.UnitTests.LoyaltyAccounts;

public class LoyaltyAccountTests
{
    [Fact]
    public void Create_ShouldInitializeAccount()
    {
        var userId = UserId.CreateUnique();
        var account = LoyaltyAccount.Create(userId);

        account.ShouldNotBeNull();
        account.UserId.ShouldBe(userId);
        account.TotalPoints.ShouldBe(0);
        account.Transactions.ShouldBeEmpty();
    }

    [Fact]
    public void EarnPoints_ShouldAddPointsAndTransaction()
    {
        var account = LoyaltyAccount.Create(UserId.CreateUnique());

        account.EarnPoints(50, "Earned from purchase");

        account.TotalPoints.ShouldBe(50);
        account.Transactions.Count.ShouldBe(1);
    }

    [Fact]
    public void RedeemPoints_WhenSufficientPoints_ShouldDeductPoints()
    {
        var account = LoyaltyAccount.Create(UserId.CreateUnique());
        account.EarnPoints(100, "Initial earn");

        var result = account.RedeemPoints(40, "Redeemed reward");

        result.IsError.ShouldBeFalse();
        account.TotalPoints.ShouldBe(60);
        account.Transactions.Count.ShouldBe(2);
    }

    [Fact]
    public void RedeemPoints_WhenInsufficientPoints_ShouldReturnError()
    {
        var account = LoyaltyAccount.Create(UserId.CreateUnique());

        var result = account.RedeemPoints(10, "Try redeem");

        result.IsError.ShouldBeTrue();
        account.TotalPoints.ShouldBe(0);
    }

    [Fact]
    public void AdjustPoints_ShouldAdjustTotalPoints()
    {
        var account = LoyaltyAccount.Create(UserId.CreateUnique());

        account.AdjustPoints(25, "Manual adjustment");

        account.TotalPoints.ShouldBe(25);
        account.Transactions.Count.ShouldBe(1);
    }

    [Fact]
    public void LoyaltyAccountId_CreateUnique_And_Create_ShouldInitialize()
    {
        var id1 = LoyaltyAccountId.CreateUnique();
        var rawGuid = Guid.NewGuid();
        var id2 = LoyaltyAccountId.Create(rawGuid);

        id1.Value.ShouldNotBe(Guid.Empty);
        id2.Value.ShouldBe(rawGuid);
    }
}

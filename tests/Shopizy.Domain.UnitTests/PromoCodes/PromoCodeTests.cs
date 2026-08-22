using Shopizy.Domain.PromoCodes;
using Shopizy.Domain.PromoCodes.ValueObjects;
using Shouldly;
using Xunit;

namespace Shopizy.Domain.UnitTests.PromoCodes;

public class PromoCodeTests
{
    [Fact]
    public void CreateAndUpdate_ShouldUpdatePromoCodeFields()
    {
        var promo = PromoCode.Create("SUMMER10", "10% off summer", 10m, true, true);

        promo.ShouldNotBeNull();
        promo.Code.ShouldBe("SUMMER10");
        promo.Description.ShouldBe("10% off summer");
        promo.Discount.ShouldBe(10m);
        promo.IsPercentage.ShouldBeTrue();
        promo.IsActive.ShouldBeTrue();
        promo.NumOfTimeUsed.ShouldBe(0);

        promo.Update("WINTER20", "20$ off winter", 20m, false, false);

        promo.Code.ShouldBe("WINTER20");
        promo.Description.ShouldBe("20$ off winter");
        promo.Discount.ShouldBe(20m);
        promo.IsPercentage.ShouldBeFalse();
        promo.IsActive.ShouldBeFalse();
    }

    [Fact]
    public void PromoCodeId_CreateUniqueAndCreate_ShouldInitialize()
    {
        var pId1 = PromoCodeId.CreateUnique();
        var raw = Guid.NewGuid();
        var pId2 = PromoCodeId.Create(raw);

        pId1.Value.ShouldNotBe(Guid.Empty);
        pId2.Value.ShouldBe(raw);
    }

    [Fact]
    public void CalculateDiscount_StandardPercentageWithCap_ShouldCapDiscount()
    {
        // 20% off with max cap of $25 on a $200 order => should be $25, not $40
        var promo = PromoCode.Create(
            "CAP20",
            "20% off up to $25",
            20m,
            true,
            true,
            Shopizy.Domain.PromoCodes.Enums.PromoType.Standard,
            maxDiscountAmount: 25m
        );

        var discount = promo.CalculateDiscount(200m);
        discount.ShouldBe(25m);
    }

    [Fact]
    public void CalculateDiscount_TieredMinimumAmount_ShouldApplyOnlyWhenThresholdMet()
    {
        var promo = PromoCode.Create(
            "TIER50",
            "$50 off over $200",
            50m,
            false,
            true,
            Shopizy.Domain.PromoCodes.Enums.PromoType.Tiered,
            minimumOrderAmount: 200m
        );

        promo.CalculateDiscount(150m).ShouldBe(0m);
        promo.CalculateDiscount(250m).ShouldBe(50m);
    }

    [Fact]
    public void CalculateDiscount_CategorySpecific_ShouldApplyOnlyToCategoryItems()
    {
        var cat1 = Shopizy.Domain.Categories.ValueObjects.CategoryId.CreateUnique();
        var cat2 = Shopizy.Domain.Categories.ValueObjects.CategoryId.CreateUnique();

        var promo = PromoCode.Create(
            "CAT10",
            "10% off Category 1",
            10m,
            true,
            true,
            Shopizy.Domain.PromoCodes.Enums.PromoType.CategorySpecific,
            targetCategoryId: cat1
        );

        var items = new List<(
            Shopizy.Domain.Categories.ValueObjects.CategoryId CategoryId,
            decimal UnitPrice,
            int Quantity
        )>
        {
            (cat1, 50m, 2), // $100 in Cat 1 => 10% = $10
            (cat2, 100m, 1), // $100 in Cat 2 => $0
        };

        var discount = promo.CalculateDiscount(200m, items);
        discount.ShouldBe(10m);
    }

    [Fact]
    public void CalculateDiscount_Bogo_Buy2Get1Free_ShouldDiscountCheapestItem()
    {
        var cat = Shopizy.Domain.Categories.ValueObjects.CategoryId.CreateUnique();

        var promo = PromoCode.Create(
            "B2G1",
            "Buy 2 Get 1 Free",
            0m,
            false,
            true,
            Shopizy.Domain.PromoCodes.Enums.PromoType.Bogo,
            buyQuantity: 2,
            getQuantity: 1,
            getDiscountPercentage: 100m
        );

        var items = new List<(
            Shopizy.Domain.Categories.ValueObjects.CategoryId CategoryId,
            decimal UnitPrice,
            int Quantity
        )>
        {
            (cat, 50m, 2), // 2 items at $50
            (cat, 30m, 1), // 1 item at $30 (cheapest => free)
        };

        var discount = promo.CalculateDiscount(130m, items);
        discount.ShouldBe(30m);
    }

    [Fact]
    public void IsValid_UsageLimitAndDates_ShouldValidateProperly()
    {
        var now = DateTime.UtcNow;

        var expiredPromo = PromoCode.Create(
            "EXPIRED",
            "Expired promo",
            10m,
            true,
            true,
            Shopizy.Domain.PromoCodes.Enums.PromoType.Standard,
            endDate: now.AddDays(-1)
        );

        expiredPromo.IsValid(100m, now, out var reason1).ShouldBeFalse();
        reason1.ShouldNotBeNull();
        reason1.ShouldContain("expired");

        var limitedPromo = PromoCode.Create(
            "LIMITED",
            "Limited uses",
            10m,
            true,
            true,
            Shopizy.Domain.PromoCodes.Enums.PromoType.Standard,
            usageLimit: 5
        );
        limitedPromo.NumOfTimeUsed = 5;

        limitedPromo.IsValid(100m, now, out var reason2).ShouldBeFalse();
        reason2.ShouldNotBeNull();
        reason2.ShouldContain("usage limit");

        var validPromo = PromoCode.Create(
            "VALID",
            "Valid promo",
            10m,
            true,
            true,
            Shopizy.Domain.PromoCodes.Enums.PromoType.Standard,
            startDate: now.AddDays(-1),
            endDate: now.AddDays(1),
            usageLimit: 10
        );

        validPromo.IsValid(100m, now, out var reason3).ShouldBeTrue();
        reason3.ShouldBeNull();
    }
}

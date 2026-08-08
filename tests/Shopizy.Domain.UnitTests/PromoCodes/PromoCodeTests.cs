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
}

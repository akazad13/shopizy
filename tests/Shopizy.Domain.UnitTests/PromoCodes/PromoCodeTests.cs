using Shouldly;
using Xunit;
using Shopizy.Domain.PromoCodes;

namespace Shopizy.Domain.UnitTests.PromoCodes;

public class PromoCodeTests
{
    [Fact]
    public void Create_WithValidData_ReturnsPromoCode()
    {
        // Arrange
        var code = "SUMMER20";
        var description = "Summer discount";
        var discount = 20.0m;
        var isPercentage = true;
        var isActive = true;

        // Act
        var promoCode = PromoCode.Create(code, description, discount, isPercentage, isActive);

        // Assert
        promoCode.ShouldNotBeNull();
        promoCode.Id.ShouldNotBeNull();
        promoCode.Id.Value.ShouldNotBe(Guid.Empty);
        promoCode.Code.ShouldBe(code);
        promoCode.Description.ShouldBe(description);
        promoCode.Discount.ShouldBe(discount);
        promoCode.IsPerchantage.ShouldBe(isPercentage);
        promoCode.IsActive.ShouldBe(isActive);
        promoCode.NumOfTimeUsed.ShouldBe(0);
    }
}

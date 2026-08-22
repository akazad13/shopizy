using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.PromoCodes.Queries.ValidatePromoCode;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.PromoCodes;

namespace Shopizy.Application.UnitTests.PromoCodes.Queries.ValidatePromoCode;

public class ValidatePromoCodeQueryHandlerTests
{
    private readonly Mock<IPromoCodeRepository> _mockRepo;
    private readonly ValidatePromoCodeQueryHandler _sut;

    public ValidatePromoCodeQueryHandlerTests()
    {
        _mockRepo = new Mock<IPromoCodeRepository>();
        _sut = new ValidatePromoCodeQueryHandler(_mockRepo.Object);
    }

    [Fact]
    public async Task Should_ReturnPromoCodeNotFound_WhenCodeDoesNotExist()
    {
        // Arrange
        var query = new ValidatePromoCodeQuery("INVALID");

        _mockRepo.Setup(x => x.GetByCodeAsync("INVALID")).ReturnsAsync((PromoCode?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(CustomErrors.PromoCode.PromoCodeNotFound, result.FirstError);
    }

    [Fact]
    public async Task Should_ReturnPromoCodeInactive_WhenCodeIsDisabled()
    {
        // Arrange
        var promoCode = PromoCode.Create("EXPIRED", "Expired offer", 10, true, false); // IsActive = false
        var query = new ValidatePromoCodeQuery("EXPIRED");

        _mockRepo.Setup(x => x.GetByCodeAsync("EXPIRED")).ReturnsAsync(promoCode);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(
            "inactive",
            result.FirstError.Description,
            StringComparison.OrdinalIgnoreCase
        );
    }

    [Fact]
    public async Task Should_ReturnError_WhenMinimumOrderSubtotalNotMet()
    {
        // Arrange
        var promoCode = PromoCode.Create(
            "TIER100",
            "Min $100",
            10,
            true,
            true,
            Shopizy.Domain.PromoCodes.Enums.PromoType.Standard,
            minimumOrderAmount: 100m
        );
        var query = new ValidatePromoCodeQuery("TIER100", OrderSubtotal: 50m);

        _mockRepo.Setup(x => x.GetByCodeAsync("TIER100")).ReturnsAsync(promoCode);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains("Minimum order amount", result.FirstError.Description);
    }

    [Fact]
    public async Task Should_ReturnPromoCode_WhenCodeIsActiveAndValid()
    {
        // Arrange
        var promoCode = PromoCode.Create("VALID20", "20% Off", 20, true, true);
        var query = new ValidatePromoCodeQuery("VALID20", OrderSubtotal: 100m);

        _mockRepo.Setup(x => x.GetByCodeAsync("VALID20")).ReturnsAsync(promoCode);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal("VALID20", result.Value.Code);
    }
}

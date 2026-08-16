using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.PromoCodes.Commands.UpdatePromoCode;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.PromoCodes;
using Shopizy.Domain.PromoCodes.ValueObjects;

namespace Shopizy.Application.UnitTests.PromoCodes.Commands.UpdatePromoCode;

public class UpdatePromoCodeCommandHandlerTests
{
    private readonly Mock<IPromoCodeRepository> _mockRepo;
    private readonly UpdatePromoCodeCommandHandler _sut;

    public UpdatePromoCodeCommandHandlerTests()
    {
        _mockRepo = new Mock<IPromoCodeRepository>();
        _sut = new UpdatePromoCodeCommandHandler(_mockRepo.Object);
    }

    [Fact]
    public async Task Should_ReturnPromoCodeNotFound_WhenPromoCodeDoesNotExist()
    {
        // Arrange
        var command = new UpdatePromoCodeCommand(
            Guid.NewGuid(),
            "NEW20",
            "20% off",
            20,
            true,
            true
        );

        _mockRepo
            .Setup(x => x.GetPromoCodeByIdAsync(It.IsAny<PromoCodeId>()))
            .ReturnsAsync((PromoCode?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(CustomErrors.PromoCode.PromoCodeNotFound, result.FirstError);
    }

    [Fact]
    public async Task Should_UpdatePromoCode_WhenPromoCodeExists()
    {
        // Arrange
        var promoCode = PromoCode.Create("OLD10", "10% off", 10, true, true);
        var command = new UpdatePromoCodeCommand(
            promoCode.Id.Value,
            "NEW20",
            "20% off",
            20,
            true,
            true
        );

        _mockRepo.Setup(x => x.GetPromoCodeByIdAsync(promoCode.Id)).ReturnsAsync(promoCode);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal("NEW20", result.Value.Code);
        Assert.Equal(20, result.Value.Discount);
    }
}

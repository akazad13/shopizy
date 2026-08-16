using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.PromoCodes.Commands.DeletePromoCode;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.PromoCodes;
using Shopizy.Domain.PromoCodes.ValueObjects;

namespace Shopizy.Application.UnitTests.PromoCodes.Commands.DeletePromoCode;

public class DeletePromoCodeCommandHandlerTests
{
    private readonly Mock<IPromoCodeRepository> _mockRepo;
    private readonly DeletePromoCodeCommandHandler _sut;

    public DeletePromoCodeCommandHandlerTests()
    {
        _mockRepo = new Mock<IPromoCodeRepository>();
        _sut = new DeletePromoCodeCommandHandler(_mockRepo.Object);
    }

    [Fact]
    public async Task Should_ReturnPromoCodeNotFound_WhenPromoCodeDoesNotExist()
    {
        // Arrange
        var command = new DeletePromoCodeCommand(Guid.NewGuid());

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
    public async Task Should_DeletePromoCode_WhenPromoCodeExists()
    {
        // Arrange
        var promoCode = PromoCode.Create("SAVE50", "50 Off", 50, false, true);
        var command = new DeletePromoCodeCommand(promoCode.Id.Value);

        _mockRepo.Setup(x => x.GetPromoCodeByIdAsync(promoCode.Id)).ReturnsAsync(promoCode);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(Result.Deleted, result.Value);
        _mockRepo.Verify(x => x.Remove(promoCode), Times.Once);
    }
}

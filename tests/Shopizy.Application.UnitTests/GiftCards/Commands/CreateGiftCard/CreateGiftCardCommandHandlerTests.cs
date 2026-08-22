using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.GiftCards.Commands.CreateGiftCard;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.GiftCards;
using Shouldly;

namespace Shopizy.Application.UnitTests.GiftCards.Commands.CreateGiftCard;

public class CreateGiftCardCommandHandlerTests
{
    private readonly Mock<IGiftCardRepository> _mockRepository;
    private readonly CreateGiftCardCommandHandler _handler;

    public CreateGiftCardCommandHandlerTests()
    {
        _mockRepository = new Mock<IGiftCardRepository>();
        _handler = new CreateGiftCardCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenCodeIsUnique_ShouldCreateAndReturnGiftCard()
    {
        // Arrange
        var command = new CreateGiftCardCommand("GIFT100", 100m, DateTime.UtcNow.AddDays(30));
        _mockRepository.Setup(r => r.GetByCodeAsync(command.Code)).ReturnsAsync((GiftCard?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.Code.ShouldBe("GIFT100");
        result.Value.InitialBalance.ShouldBe(100m);
        result.Value.RemainingBalance.ShouldBe(100m);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<GiftCard>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCodeAlreadyExists_ShouldReturnDuplicateCodeError()
    {
        // Arrange
        var existing = GiftCard.Create("GIFT100", 50m, null);
        var command = new CreateGiftCardCommand("GIFT100", 100m, null);
        _mockRepository.Setup(r => r.GetByCodeAsync(command.Code)).ReturnsAsync(existing);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe((Error)CustomErrors.GiftCard.DuplicateCode);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<GiftCard>()), Times.Never);
    }
}

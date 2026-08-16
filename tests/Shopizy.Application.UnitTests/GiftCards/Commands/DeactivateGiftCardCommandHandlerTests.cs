using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.GiftCards.Commands.DeactivateGiftCard;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.GiftCards;
using Shopizy.Domain.GiftCards.ValueObjects;
using Shouldly;
using Xunit;

namespace Shopizy.Application.UnitTests.GiftCards.Commands;

public class DeactivateGiftCardCommandHandlerTests
{
    private readonly Mock<IGiftCardRepository> _mockGiftCardRepository;
    private readonly DeactivateGiftCardCommandHandler _sut;

    public DeactivateGiftCardCommandHandlerTests()
    {
        _mockGiftCardRepository = new Mock<IGiftCardRepository>();
        _sut = new DeactivateGiftCardCommandHandler(_mockGiftCardRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenGiftCardNotFound_ReturnsNotFound()
    {
        // Arrange
        var giftCardId = GiftCardId.CreateUnique();
        var command = new DeactivateGiftCardCommand(giftCardId);
        _mockGiftCardRepository
            .Setup(x => x.GetByIdAsync(giftCardId))
            .ReturnsAsync((GiftCard?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe((Error)CustomErrors.GiftCard.GiftCardNotFound);
    }

    [Fact]
    public async Task Handle_WhenGiftCardExists_DeactivatesCardAndUpdateRepository()
    {
        // Arrange
        var giftCard = GiftCard.Create("ACTIVE-CODE", 100.00m, null);
        var command = new DeactivateGiftCardCommand(giftCard.Id);

        _mockGiftCardRepository.Setup(x => x.GetByIdAsync(giftCard.Id)).ReturnsAsync(giftCard);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.IsActive.ShouldBeFalse();
        _mockGiftCardRepository.Verify(x => x.Update(giftCard), Times.Once);
    }
}

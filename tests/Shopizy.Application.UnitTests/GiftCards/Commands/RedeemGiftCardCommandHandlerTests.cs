using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.GiftCards.Commands.RedeemGiftCard;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.GiftCards;
using Shopizy.Domain.Users.ValueObjects;
using Shouldly;
using Xunit;

namespace Shopizy.Application.UnitTests.GiftCards.Commands;

public class RedeemGiftCardCommandHandlerTests
{
    private readonly Mock<IGiftCardRepository> _mockGiftCardRepository;
    private readonly RedeemGiftCardCommandHandler _sut;

    public RedeemGiftCardCommandHandlerTests()
    {
        _mockGiftCardRepository = new Mock<IGiftCardRepository>();
        _sut = new RedeemGiftCardCommandHandler(_mockGiftCardRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenGiftCardNotFound_ReturnsNotFound()
    {
        // Arrange
        var command = new RedeemGiftCardCommand(UserId.CreateUnique(), "NONEXISTENT");
        _mockGiftCardRepository
            .Setup(x => x.GetByCodeAsync(command.Code))
            .ReturnsAsync((GiftCard?)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe((Error)CustomErrors.GiftCard.GiftCardNotFound);
    }

    [Fact]
    public async Task Handle_WhenGiftCardIsValid_RedeemsGiftCardAndUpdateRepository()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        var giftCard = GiftCard.Create("VALID-CODE", 100.00m, DateTime.UtcNow.AddDays(30));
        var command = new RedeemGiftCardCommand(userId, "VALID-CODE");

        _mockGiftCardRepository.Setup(x => x.GetByCodeAsync("VALID-CODE")).ReturnsAsync(giftCard);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.RedeemedByUserId.ShouldBe(userId);
        result.Value.IsActive.ShouldBeFalse();
        _mockGiftCardRepository.Verify(x => x.Update(giftCard), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenGiftCardIsExpired_ReturnsExpiredError()
    {
        // Arrange
        var userId = UserId.CreateUnique();
        var giftCard = GiftCard.Create("EXPIRED-CODE", 50.00m, DateTime.UtcNow.AddDays(-1));
        var command = new RedeemGiftCardCommand(userId, "EXPIRED-CODE");

        _mockGiftCardRepository.Setup(x => x.GetByCodeAsync("EXPIRED-CODE")).ReturnsAsync(giftCard);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe((Error)CustomErrors.GiftCard.GiftCardExpired);
    }

    [Fact]
    public async Task Handle_WhenGiftCardIsAlreadyRedeemed_ReturnsAlreadyRedeemedError()
    {
        // Arrange
        var userId1 = UserId.CreateUnique();
        var userId2 = UserId.CreateUnique();
        var giftCard = GiftCard.Create("REDEEMED-CODE", 50.00m, null);
        giftCard.Redeem(userId1);

        var command = new RedeemGiftCardCommand(userId2, "REDEEMED-CODE");

        _mockGiftCardRepository
            .Setup(x => x.GetByCodeAsync("REDEEMED-CODE"))
            .ReturnsAsync(giftCard);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe((Error)CustomErrors.GiftCard.GiftCardAlreadyRedeemed);
    }
}

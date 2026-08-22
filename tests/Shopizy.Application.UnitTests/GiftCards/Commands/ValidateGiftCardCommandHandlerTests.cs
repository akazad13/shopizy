using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.GiftCards.Commands.ValidateGiftCard;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.GiftCards;
using Shouldly;
using Xunit;

namespace Shopizy.Application.UnitTests.GiftCards.Commands;

public class ValidateGiftCardCommandHandlerTests
{
    private readonly Mock<IGiftCardRepository> _mockGiftCardRepository;
    private readonly ValidateGiftCardCommandHandler _handler;

    public ValidateGiftCardCommandHandlerTests()
    {
        _mockGiftCardRepository = new Mock<IGiftCardRepository>();
        _handler = new ValidateGiftCardCommandHandler(_mockGiftCardRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenNotFound_ShouldReturnGiftCardNotFound()
    {
        _mockGiftCardRepository
            .Setup(r => r.GetByCodeAsync("INVALID"))
            .ReturnsAsync((GiftCard?)null);

        var result = await _handler.Handle(
            new ValidateGiftCardCommand("INVALID"),
            CancellationToken.None
        );

        result.IsError.ShouldBeTrue();
        result.FirstError.Code.ShouldBe(CustomErrors.GiftCard.GiftCardNotFound.Code);
    }

    [Fact]
    public async Task Handle_WhenInactive_ShouldReturnGiftCardInactive()
    {
        var card = GiftCard.Create("INACTIVE", 100, DateTime.UtcNow.AddDays(10));
        card.Deactivate();

        _mockGiftCardRepository.Setup(r => r.GetByCodeAsync("INACTIVE")).ReturnsAsync(card);

        var result = await _handler.Handle(
            new ValidateGiftCardCommand("INACTIVE"),
            CancellationToken.None
        );

        result.IsError.ShouldBeTrue();
        result.FirstError.Code.ShouldBe(CustomErrors.GiftCard.GiftCardInactive.Code);
    }

    [Fact]
    public async Task Handle_WhenExpired_ShouldReturnGiftCardExpired()
    {
        var card = GiftCard.Create("EXPIRED", 100, DateTime.UtcNow.AddDays(-1));

        _mockGiftCardRepository.Setup(r => r.GetByCodeAsync("EXPIRED")).ReturnsAsync(card);

        var result = await _handler.Handle(
            new ValidateGiftCardCommand("EXPIRED"),
            CancellationToken.None
        );

        result.IsError.ShouldBeTrue();
        result.FirstError.Code.ShouldBe(CustomErrors.GiftCard.GiftCardExpired.Code);
    }

    [Fact]
    public async Task Handle_WhenValid_ShouldReturnGiftCard()
    {
        var card = GiftCard.Create("VALID", 100, DateTime.UtcNow.AddDays(10));

        _mockGiftCardRepository.Setup(r => r.GetByCodeAsync("VALID")).ReturnsAsync(card);

        var result = await _handler.Handle(
            new ValidateGiftCardCommand("VALID"),
            CancellationToken.None
        );

        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(card);
    }
}

using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.LoyaltyAccounts.Commands.RedeemPoints;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.LoyaltyAccounts;
using Shopizy.Domain.Users.ValueObjects;
using Shouldly;
using Xunit;

namespace Shopizy.Application.UnitTests.LoyaltyAccounts.Commands.RedeemPoints;

public class RedeemPointsCommandHandlerTests
{
    private readonly Mock<ILoyaltyAccountRepository> _mockLoyaltyAccountRepository;
    private readonly RedeemPointsCommandHandler _handler;

    public RedeemPointsCommandHandlerTests()
    {
        _mockLoyaltyAccountRepository = new Mock<ILoyaltyAccountRepository>();
        _handler = new RedeemPointsCommandHandler(_mockLoyaltyAccountRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenAccountNotFound_ShouldReturnAccountNotFound()
    {
        _mockLoyaltyAccountRepository
            .Setup(r => r.GetByUserIdAsync(It.IsAny<UserId>()))
            .ReturnsAsync((LoyaltyAccount?)null);

        var result = await _handler.Handle(
            new RedeemPointsCommand(Guid.NewGuid(), 100, "Redeem"),
            CancellationToken.None
        );

        result.IsError.ShouldBeTrue();
        result.FirstError.Code.ShouldBe(CustomErrors.LoyaltyAccount.AccountNotFound.Code);
    }

    [Fact]
    public async Task Handle_WhenInsufficientPoints_ShouldReturnError()
    {
        var userId = UserId.CreateUnique();
        var account = LoyaltyAccount.Create(userId); // starts with 0 points

        _mockLoyaltyAccountRepository
            .Setup(r => r.GetByUserIdAsync(It.IsAny<UserId>()))
            .ReturnsAsync(account);

        var result = await _handler.Handle(
            new RedeemPointsCommand(userId.Value, 50, "Redeem"),
            CancellationToken.None
        );

        result.IsError.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_WhenSufficientPoints_ShouldRedeemAndReturnAccount()
    {
        var userId = UserId.CreateUnique();
        var account = LoyaltyAccount.Create(userId);
        account.EarnPoints(100, "Earned from purchase");

        _mockLoyaltyAccountRepository
            .Setup(r => r.GetByUserIdAsync(It.IsAny<UserId>()))
            .ReturnsAsync(account);

        var result = await _handler.Handle(
            new RedeemPointsCommand(userId.Value, 50, "Redeem"),
            CancellationToken.None
        );

        result.IsError.ShouldBeFalse();
        result.Value.TotalPoints.ShouldBe(50);
    }
}

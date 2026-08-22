using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.LoyaltyAccounts.Queries.GetLoyaltyAccount;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.LoyaltyAccounts;
using Shopizy.Domain.Users.ValueObjects;
using Shouldly;
using Xunit;

namespace Shopizy.Application.UnitTests.LoyaltyAccounts.Queries.GetLoyaltyAccount;

public class GetLoyaltyAccountQueryHandlerTests
{
    private readonly Mock<ILoyaltyAccountRepository> _mockLoyaltyAccountRepository;
    private readonly GetLoyaltyAccountQueryHandler _handler;

    public GetLoyaltyAccountQueryHandlerTests()
    {
        _mockLoyaltyAccountRepository = new Mock<ILoyaltyAccountRepository>();
        _handler = new GetLoyaltyAccountQueryHandler(_mockLoyaltyAccountRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenAccountNotFound_ShouldReturnAccountNotFound()
    {
        _mockLoyaltyAccountRepository
            .Setup(r => r.GetByUserIdAsync(It.IsAny<UserId>()))
            .ReturnsAsync((LoyaltyAccount?)null);

        var result = await _handler.Handle(
            new GetLoyaltyAccountQuery(Guid.NewGuid()),
            CancellationToken.None
        );

        result.IsError.ShouldBeTrue();
        result.FirstError.Code.ShouldBe(CustomErrors.LoyaltyAccount.AccountNotFound.Code);
    }

    [Fact]
    public async Task Handle_WhenAccountFound_ShouldReturnAccount()
    {
        var userId = UserId.CreateUnique();
        var account = LoyaltyAccount.Create(userId);

        _mockLoyaltyAccountRepository
            .Setup(r => r.GetByUserIdAsync(It.Is<UserId>(u => u.Value == userId.Value)))
            .ReturnsAsync(account);

        var result = await _handler.Handle(
            new GetLoyaltyAccountQuery(userId.Value),
            CancellationToken.None
        );

        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(account);
    }
}

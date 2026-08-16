using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.LoyaltyAccounts.Events;
using Shopizy.Application.UnitTests.Orders.TestUtils;
using Shopizy.Domain.LoyaltyAccounts;
using Shopizy.Domain.Orders.Events;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Application.Interfaces.Persistence;
using Shouldly;

namespace Shopizy.Application.UnitTests.LoyaltyAccounts.Events;

public class PaymentCompletedLoyaltyEventHandlerTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<ILoyaltyAccountRepository> _mockLoyaltyAccountRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly PaymentCompletedLoyaltyEventHandler _handler;

    public PaymentCompletedLoyaltyEventHandlerTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockLoyaltyAccountRepository = new Mock<ILoyaltyAccountRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        _handler = new PaymentCompletedLoyaltyEventHandler(
            _mockOrderRepository.Object,
            _mockLoyaltyAccountRepository.Object,
            _mockUnitOfWork.Object
        );
    }

    [Fact]
    public async Task Handle_WhenOrderNotFound_ShouldReturnWithoutSavingChanges()
    {
        // Arrange
        var domainEvent = new PaymentCompletedDomainEvent(
            OrderId.Create(Guid.NewGuid()),
            UserId.Create(Guid.NewGuid()),
            "cus_123"
        );

        _mockOrderRepository
            .Setup(r => r.GetOrderByIdAsync(It.IsAny<OrderId>()))
            .ReturnsAsync((Shopizy.Domain.Orders.Order?)null);

        // Act
        await _handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        _mockLoyaltyAccountRepository.Verify(
            r => r.GetByUserIdAsync(It.IsAny<UserId>()),
            Times.Never
        );
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenOrderTotalIsZeroOrLess_ShouldReturnWithoutSavingChanges()
    {
        // Arrange
        var order = OrderFactory.CreateOrder();
        // OrderFactory creates an order with delivery charge. Let's assume delivery charge is > 0,
        // but let's mock it if possible. Wait, we can't easily mock order total without reflection,
        // but we can create a specific order. Let's use the factory but reflection to change delivery charge to 0.
        var property = typeof(Shopizy.Domain.Orders.Order).GetProperty("DeliveryCharge");
        property?.SetValue(
            order,
            Shopizy.Domain.Common.ValueObjects.Price.CreateNew(
                0,
                Shopizy.Domain.Common.Enums.Currency.usd
            )
        );

        var domainEvent = new PaymentCompletedDomainEvent(order.Id, order.UserId, "cus_123");

        _mockOrderRepository.Setup(r => r.GetOrderByIdAsync(order.Id)).ReturnsAsync(order);

        // Act
        await _handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        _mockLoyaltyAccountRepository.Verify(
            r => r.GetByUserIdAsync(It.IsAny<UserId>()),
            Times.Never
        );
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenOrderTotalIsPositive_ShouldEarnPointsAndSaveChanges()
    {
        // Arrange
        var order = OrderFactory.CreateOrder();
        var domainEvent = new PaymentCompletedDomainEvent(order.Id, order.UserId, "cus_123");

        _mockOrderRepository.Setup(r => r.GetOrderByIdAsync(order.Id)).ReturnsAsync(order);

        var loyaltyAccount = LoyaltyAccount.Create(order.UserId);
        _mockLoyaltyAccountRepository
            .Setup(r => r.GetByUserIdAsync(order.UserId))
            .ReturnsAsync(loyaltyAccount);

        // Act
        await _handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        loyaltyAccount.TotalPoints.ShouldBeGreaterThan(0);
        _mockLoyaltyAccountRepository.Verify(
            r => r.AddAsync(It.IsAny<LoyaltyAccount>()),
            Times.Never
        );
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenLoyaltyAccountDoesNotExist_ShouldCreateAccountAndEarnPoints()
    {
        // Arrange
        var order = OrderFactory.CreateOrder();
        var domainEvent = new PaymentCompletedDomainEvent(order.Id, order.UserId, "cus_123");

        _mockOrderRepository.Setup(r => r.GetOrderByIdAsync(order.Id)).ReturnsAsync(order);

        _mockLoyaltyAccountRepository
            .Setup(r => r.GetByUserIdAsync(order.UserId))
            .ReturnsAsync((LoyaltyAccount?)null);

        // Act
        await _handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        _mockLoyaltyAccountRepository.Verify(
            r => r.AddAsync(It.IsAny<LoyaltyAccount>()),
            Times.Once
        );
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

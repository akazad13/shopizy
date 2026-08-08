using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Payments.Events;
using Shopizy.Application.UnitTests.Orders.TestUtils;
using Shopizy.Application.UnitTests.Users.TestUtils;
using Shopizy.Domain.Orders.Events;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Application.Interfaces.Persistence;
using Shouldly;

namespace Shopizy.Application.UnitTests.Payments.Events;

public class PaymentCompletedDomainEventHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly PaymentCompletedDomainEventHandler _handler;

    public PaymentCompletedDomainEventHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        _handler = new PaymentCompletedDomainEventHandler(
            _mockUserRepository.Object,
            _mockUnitOfWork.Object
        );
    }

    [Fact]
    public async Task Handle_WhenCustomerIdIsEmpty_ShouldReturnImmediatelyWithoutSavingChanges()
    {
        // Arrange
        var domainEvent = new PaymentCompletedDomainEvent(
            OrderId.Create(Guid.NewGuid()),
            UserId.Create(Guid.NewGuid()),
            string.Empty
        );

        // Act
        await _handler.Handle(domainEvent, TestContext.Current.CancellationToken);

        // Assert
        _mockUserRepository.Verify(r => r.GetUserByIdAsync(It.IsAny<UserId>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenCustomerIdIsNull_ShouldReturnImmediatelyWithoutSavingChanges()
    {
        // Arrange
        var domainEvent = new PaymentCompletedDomainEvent(
            OrderId.Create(Guid.NewGuid()),
            UserId.Create(Guid.NewGuid()),
            null!
        );

        // Act
        await _handler.Handle(domainEvent, TestContext.Current.CancellationToken);

        // Assert
        _mockUserRepository.Verify(r => r.GetUserByIdAsync(It.IsAny<UserId>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldReturnWithoutSavingChanges()
    {
        // Arrange
        var domainEvent = new PaymentCompletedDomainEvent(
            OrderId.Create(Guid.NewGuid()),
            UserId.Create(Guid.NewGuid()),
            "cus_123"
        );

        _mockUserRepository
            .Setup(r => r.GetUserByIdAsync(It.IsAny<UserId>()))
            .ReturnsAsync((Shopizy.Domain.Users.User?)null);

        // Act
        await _handler.Handle(domainEvent, TestContext.Current.CancellationToken);

        // Assert
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserFound_ShouldUpdateCustomerIdAndSaveChanges()
    {
        // Arrange
        const string customerId = "cus_456";
        var user = UserFactory.CreateUser();
        var domainEvent = new PaymentCompletedDomainEvent(
            OrderId.Create(Guid.NewGuid()),
            UserId.Create(Guid.NewGuid()),
            customerId
        );

        _mockUserRepository.Setup(r => r.GetUserByIdAsync(It.IsAny<UserId>())).ReturnsAsync(user);
        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(domainEvent, TestContext.Current.CancellationToken);

        // Assert
        user.CustomerId.ShouldBe(customerId);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

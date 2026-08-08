using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.UnitTests.Users.TestUtils;
using Shopizy.Application.Users.Events;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Users.Events;
using Shopizy.SharedKernel.Application.Interfaces.Persistence;

namespace Shopizy.Application.UnitTests.Users.Events;

public class UserRegisteredDomainEventHandlerTests
{
    private readonly Mock<ICartRepository> _mockCartRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly UserRegisteredDomainEventHandler _handler;

    public UserRegisteredDomainEventHandlerTests()
    {
        _mockCartRepository = new Mock<ICartRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        _handler = new UserRegisteredDomainEventHandler(
            _mockCartRepository.Object,
            _mockUnitOfWork.Object
        );
    }

    [Fact]
    public async Task Handle_WhenUserRegistered_ShouldCreateCartAndSaveChanges()
    {
        // Arrange
        var user = UserFactory.CreateUser();
        var domainEvent = new UserRegisteredDomainEvent(user);

        _mockCartRepository.Setup(c => c.AddAsync(It.IsAny<Cart>())).Returns(Task.CompletedTask);
        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(domainEvent, TestContext.Current.CancellationToken);

        // Assert
        _mockCartRepository.Verify(
            c => c.AddAsync(It.Is<Cart>(cart => cart.UserId == user.Id)),
            Times.Once
        );
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

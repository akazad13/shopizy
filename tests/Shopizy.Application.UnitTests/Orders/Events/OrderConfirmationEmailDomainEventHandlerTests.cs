using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Orders.Events;
using Shopizy.Application.UnitTests.Orders.TestUtils;
using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Domain.Orders.Events;
using Shopizy.Domain.Users;
using Shopizy.Domain.Users.Enums;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.UnitTests.Orders.Events;

public class OrderConfirmationEmailDomainEventHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly OrderConfirmationEmailDomainEventHandler _sut;

    public OrderConfirmationEmailDomainEventHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockEmailService = new Mock<IEmailService>();

        _sut = new OrderConfirmationEmailDomainEventHandler(
            _mockUserRepository.Object,
            _mockEmailService.Object
        );
    }

    [Fact]
    public async Task Should_DoNothing_WhenUserNotFound()
    {
        // Arrange
        var order = OrderFactory.CreateOrder();
        var domainEvent = new OrderCreatedDomainEvent(order);

        _mockUserRepository
            .Setup(x => x.GetUserByIdAsync(It.IsAny<UserId>()))
            .ReturnsAsync((User?)null);

        // Act
        await _sut.Handle(domainEvent, CancellationToken.None);

        // Assert
        _mockEmailService.Verify(
            x =>
                x.SendAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Never
        );
    }

    [Fact]
    public async Task Should_SendEmail_WhenUserIsFound()
    {
        // Arrange
        var order = OrderFactory.CreateOrder();
        var user = User.Create(
            Constants.User.FirstName,
            Constants.User.LastName,
            Constants.User.Email,
            Constants.User.Password,
            UserRole.Customer,
            []
        );

        var domainEvent = new OrderCreatedDomainEvent(order);

        _mockUserRepository.Setup(x => x.GetUserByIdAsync(order.UserId)).ReturnsAsync(user);

        // Act
        await _sut.Handle(domainEvent, CancellationToken.None);

        // Assert
        _mockEmailService.Verify(
            x =>
                x.SendAsync(
                    user.Email,
                    It.Is<string>(s => s.Contains(order.Id.Value.ToString())),
                    It.Is<string>(b =>
                        b.Contains(user.FirstName) && b.Contains(order.Id.Value.ToString())
                    ),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }
}

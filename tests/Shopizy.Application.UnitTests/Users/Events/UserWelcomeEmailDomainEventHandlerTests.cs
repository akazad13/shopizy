using Moq;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.UnitTests.Users.TestUtils;
using Shopizy.Application.Users.Events;
using Shopizy.Domain.Users.Events;

namespace Shopizy.Application.UnitTests.Users.Events;

public class UserWelcomeEmailDomainEventHandlerTests
{
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly UserWelcomeEmailDomainEventHandler _handler;

    public UserWelcomeEmailDomainEventHandlerTests()
    {
        _mockEmailService = new Mock<IEmailService>();
        _handler = new UserWelcomeEmailDomainEventHandler(_mockEmailService.Object);
    }

    [Fact]
    public async Task Handle_WhenUserRegistered_ShouldSendWelcomeEmail()
    {
        // Arrange
        var user = UserFactory.CreateUser();
        var domainEvent = new UserRegisteredDomainEvent(user);

        _mockEmailService
            .Setup(e =>
                e.SendAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(domainEvent, TestContext.Current.CancellationToken);

        // Assert
        _mockEmailService.Verify(
            e =>
                e.SendAsync(
                    user.Email,
                    "Welcome to Shopizy!",
                    It.Is<string>(body => body.Contains(user.FirstName)),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }
}

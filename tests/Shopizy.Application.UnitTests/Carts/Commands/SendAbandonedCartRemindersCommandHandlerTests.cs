using ErrorOr;
using Microsoft.Extensions.Logging;
using Moq;
using Shopizy.Application.Carts.Commands.SendAbandonedCartReminders;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.UnitTests.Users.TestUtils;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Carts.Entities;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Application.Interfaces.Persistence;
using Shouldly;

namespace Shopizy.Application.UnitTests.Carts.Commands;

public class SendAbandonedCartRemindersCommandHandlerTests
{
    private readonly Mock<ICartRepository> _mockCartRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ILogger<SendAbandonedCartRemindersCommandHandler>> _mockLogger;
    private readonly SendAbandonedCartRemindersCommandHandler _handler;

    public SendAbandonedCartRemindersCommandHandlerTests()
    {
        _mockCartRepository = new Mock<ICartRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockEmailService = new Mock<IEmailService>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockLogger = new Mock<ILogger<SendAbandonedCartRemindersCommandHandler>>();

        _handler = new SendAbandonedCartRemindersCommandHandler(
            _mockCartRepository.Object,
            _mockUserRepository.Object,
            _mockEmailService.Object,
            _mockUnitOfWork.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task Handle_WhenNoAbandonedCarts_ShouldReturnZero()
    {
        // Arrange
        var threshold = DateTime.UtcNow.AddHours(-2);
        var command = new SendAbandonedCartRemindersCommand(threshold);

        _mockCartRepository
            .Setup(r => r.GetAbandonedCartsAsync(threshold, 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Cart>());

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(0);
        _mockEmailService.Verify(
            e =>
                e.SendAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Never
        );
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenAbandonedCartsExist_ShouldSendEmailsAndRecordTimestamp()
    {
        // Arrange
        var threshold = DateTime.UtcNow.AddHours(-2);
        var command = new SendAbandonedCartRemindersCommand(threshold, 10);

        var user = UserFactory.CreateUser();
        var cart = Cart.Create(user.Id);
        cart.AddLineItem(CartItem.Create(ProductId.CreateUnique(), "Blue", "M", 2));

        _mockCartRepository
            .Setup(r => r.GetAbandonedCartsAsync(threshold, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Cart> { cart });

        _mockUserRepository.Setup(r => r.GetUserByIdAsync(user.Id)).ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(1);

        cart.LastAbandonedReminderSentOn.ShouldNotBeNull();
        _mockEmailService.Verify(
            e =>
                e.SendAsync(
                    user.Email,
                    It.Is<string>(s => s.Contains("left items in your cart")),
                    It.Is<string>(b => b.Contains("2 item(s)")),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );

        _mockCartRepository.Verify(r => r.Update(cart), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenEmailSendThrows_ShouldContinueAndStillRecordTimestamp()
    {
        // Arrange
        var threshold = DateTime.UtcNow.AddHours(-2);
        var command = new SendAbandonedCartRemindersCommand(threshold, 10);

        var user = UserFactory.CreateUser();
        var cart = Cart.Create(user.Id);
        cart.AddLineItem(CartItem.Create(ProductId.CreateUnique(), "Black", "L", 1));

        _mockCartRepository
            .Setup(r => r.GetAbandonedCartsAsync(threshold, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Cart> { cart });

        _mockUserRepository.Setup(r => r.GetUserByIdAsync(user.Id)).ReturnsAsync(user);

        _mockEmailService
            .Setup(e =>
                e.SendAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new InvalidOperationException("SMTP connection error"));

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.ShouldBe(0); // 0 successful sends
        cart.LastAbandonedReminderSentOn.ShouldNotBeNull(); // still recorded to avoid retry hammering
        _mockCartRepository.Verify(r => r.Update(cart), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

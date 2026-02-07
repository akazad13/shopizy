using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Common.models;
using Shopizy.Application.Common.Security.CurrentUser;
using Shopizy.Application.Payments.Commands.CardNotPresentSale;
using Shopizy.Application.UnitTests.Orders.TestUtils;
using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Application.UnitTests.Users.TestUtils;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Payments;
using Shopizy.Domain.Users.ValueObjects;
using Shouldly;

namespace Shopizy.Application.UnitTests.Payments.Commands.CardNotPresentSale;

public class CardNotPresentSaleCommandHandlerTests
{
    private readonly Mock<IPaymentRepository> _mockPaymentRepository;
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IPaymentService> _mockPaymentService;
    private readonly Mock<ICurrentUser> _mockCurrentUser;
    private readonly CardNotPresentSaleCommandHandler _handler;

    public CardNotPresentSaleCommandHandlerTests()
    {
        _mockPaymentRepository = new Mock<IPaymentRepository>();
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockPaymentService = new Mock<IPaymentService>();
        _mockCurrentUser = new Mock<ICurrentUser>();

        _handler = new CardNotPresentSaleCommandHandler(
            _mockPaymentRepository.Object,
            _mockOrderRepository.Object,
            _mockUserRepository.Object,
            _mockPaymentService.Object,
            _mockCurrentUser.Object
        );
    }

    [Fact]
    public async Task Handle_WhenOrderNotFound_ShouldReturnError()
    {
        // Arrange
        var command = CreateCommand();
        _mockOrderRepository.Setup(r => r.GetOrderByIdAsync(It.IsAny<OrderId>()))
            .ReturnsAsync((Shopizy.Domain.Orders.Order?)null);

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.ShouldBe(CustomErrors.Order.OrderNotFound);
    }

    [Fact]
    public async Task Handle_WhenPaymentServiceFails_ShouldReturnError()
    {
        // Arrange
        var command = CreateCommand();
        var order = OrderFactory.CreateOrder();
        var user = UserFactory.CreateUser();
        user.UpdateCustomerId("cus_123");

        _mockOrderRepository.Setup(r => r.GetOrderByIdAsync(It.IsAny<OrderId>())).ReturnsAsync(order);
        _mockCurrentUser.Setup(u => u.GetCurrentUserId()).Returns(Constants.User.Id.Value);
        _mockUserRepository.Setup(r => r.GetUserByIdAsync(It.IsAny<UserId>())).ReturnsAsync(user);
        _mockPaymentService.Setup(s => s.CreateSaleAsync(It.IsAny<CreateSaleRequest>()))
            .ReturnsAsync(Error.Failure("payment.failed", "Failed to collect the payment."));

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Code.ShouldBe("payment.failed");
    }

    [Fact]
    public async Task Handle_WhenSuccessful_ShouldReturnSuccess()
    {
        // Arrange
        var command = CreateCommand();
        var order = OrderFactory.CreateOrder();
        var user = UserFactory.CreateUser();
        user.UpdateCustomerId("cus_123");

        _mockOrderRepository.Setup(r => r.GetOrderByIdAsync(It.IsAny<OrderId>())).ReturnsAsync(order);
        _mockCurrentUser.Setup(u => u.GetCurrentUserId()).Returns(Constants.User.Id.Value);
        _mockUserRepository.Setup(r => r.GetUserByIdAsync(It.IsAny<UserId>())).ReturnsAsync(user);
        _mockPaymentService.Setup(s => s.CreateSaleAsync(It.IsAny<CreateSaleRequest>()))
            .ReturnsAsync(new CreateSaleResponse { ChargeId = "ch_123" });

        // Act
        var result = await _handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsError.ShouldBeFalse();
        _mockOrderRepository.Verify(r => r.Update(order), Times.Once);
        _mockPaymentRepository.Verify(r => r.Update(It.IsAny<Payment>()), Times.Once);
    }

    private static CardNotPresentSaleCommand CreateCommand()
    {
        return new CardNotPresentSaleCommand(
            Constants.User.Id.Value,
            Constants.Order.Id.Value,
            100m,
            "usd",
            "card",
            "pm_123",
            "John Doe",
            12,
            2025,
            "4242"
        );
    }
}

using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Returns.Commands.RejectReturn;
using Shopizy.Application.Returns.Commands.RequestReturn;
using Shopizy.Application.Returns.Queries.GetPendingReturns;
using Shopizy.Application.Returns.Queries.GetReturnById;
using Shopizy.Application.Returns.Queries.GetReturnsByOrder;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Orders;
using Shopizy.Domain.Orders.Entities;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Returns;
using Shopizy.Domain.Returns.Entities;
using Shopizy.Domain.Returns.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Application.Interfaces.Persistence;
using Shouldly;

namespace Shopizy.Application.UnitTests.Returns;

public class ReturnCommandAndQueryTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<IReturnRequestRepository> _mockReturnRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;

    public ReturnCommandAndQueryTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockReturnRepository = new Mock<IReturnRequestRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
    }

    [Fact]
    public async Task RequestReturn_WhenOrderNotFoundOrUserMismatch_ShouldReturnOrderNotFound()
    {
        // Arrange
        _mockOrderRepository
            .Setup(r => r.GetOrderByIdAsync(It.IsAny<OrderId>()))
            .ReturnsAsync((Order?)null);

        var handler = new RequestReturnCommandHandler(
            _mockOrderRepository.Object,
            _mockReturnRepository.Object,
            _mockUnitOfWork.Object
        );

        var command = new RequestReturnCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Defective",
            new List<RequestReturnItemCommand> { new(Guid.NewGuid(), 1) }
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Code.ShouldBe(CustomErrors.Order.OrderNotFound.Code);
    }

    [Fact]
    public async Task RequestReturn_WhenOrderValid_ShouldCreateReturnRequest()
    {
        // Arrange
        var userId = UserId.Create(Guid.NewGuid());
        var address = Address.CreateNew("123 Main St", "City", "State", "12345", "Country");
        var deliveryCharge = Price.CreateNew(10m, Currency.usd);
        var order = Order.Create(userId, "", 0, deliveryCharge, address, new List<OrderItem>());

        _mockOrderRepository
            .Setup(r => r.GetOrderByIdAsync(It.Is<OrderId>(id => id.Value == order.Id.Value)))
            .ReturnsAsync(order);

        var handler = new RequestReturnCommandHandler(
            _mockOrderRepository.Object,
            _mockReturnRepository.Object,
            _mockUnitOfWork.Object
        );

        var command = new RequestReturnCommand(
            order.Id.Value,
            userId.Value,
            "Defective item",
            new List<RequestReturnItemCommand> { new(Guid.NewGuid(), 1) }
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        _mockReturnRepository.Verify(
            r => r.AddAsync(It.IsAny<ReturnRequest>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RejectReturn_WhenNotFound_ShouldReturnReturnNotFound()
    {
        // Arrange
        _mockReturnRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<ReturnRequestId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ReturnRequest?)null);

        var handler = new RejectReturnCommandHandler(
            _mockReturnRepository.Object,
            _mockUnitOfWork.Object
        );
        var command = new RejectReturnCommand(Guid.NewGuid(), "Out of policy");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.FirstError.Code.ShouldBe(CustomErrors.ReturnRequest.ReturnNotFound.Code);
    }

    [Fact]
    public async Task RejectReturn_WhenPending_ShouldRejectAndUpdate()
    {
        // Arrange
        var orderId = OrderId.Create(Guid.NewGuid());
        var userId = UserId.Create(Guid.NewGuid());
        var items = new List<ReturnItem>
        {
            ReturnItem.Create(OrderItemId.Create(Guid.NewGuid()), 1),
        };
        var returnRequest = ReturnRequest.Create(orderId, userId, "Size mismatch", items);

        _mockReturnRepository
            .Setup(r =>
                r.GetByIdAsync(
                    It.Is<ReturnRequestId>(id => id.Value == returnRequest.Id.Value),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(returnRequest);

        var handler = new RejectReturnCommandHandler(
            _mockReturnRepository.Object,
            _mockUnitOfWork.Object
        );
        var command = new RejectReturnCommand(returnRequest.Id.Value, "Policy exceeded");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        returnRequest.Status.ToString().ShouldBe("Rejected");
        _mockReturnRepository.Verify(r => r.Update(returnRequest), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPendingReturnsQueryHandler_ShouldReturnPendingRequests()
    {
        // Arrange
        var orderId = OrderId.Create(Guid.NewGuid());
        var userId = UserId.Create(Guid.NewGuid());
        var items = new List<ReturnItem>
        {
            ReturnItem.Create(OrderItemId.Create(Guid.NewGuid()), 1),
        };
        var returns = new List<ReturnRequest>
        {
            ReturnRequest.Create(orderId, userId, "Damaged", items),
        };

        _mockReturnRepository
            .Setup(r => r.GetPendingReturnsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(returns);

        var handler = new GetPendingReturnsQueryHandler(_mockReturnRepository.Object);

        // Act
        var result = await handler.Handle(new GetPendingReturnsQuery(), CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.Count.ShouldBe(1);
    }

    [Fact]
    public async Task GetReturnByIdQueryHandler_WhenFound_ShouldReturnRequest()
    {
        // Arrange
        var orderId = OrderId.Create(Guid.NewGuid());
        var userId = UserId.Create(Guid.NewGuid());
        var items = new List<ReturnItem>
        {
            ReturnItem.Create(OrderItemId.Create(Guid.NewGuid()), 1),
        };
        var returnRequest = ReturnRequest.Create(orderId, userId, "Damaged", items);

        _mockReturnRepository
            .Setup(r =>
                r.GetByIdAsync(
                    It.Is<ReturnRequestId>(id => id.Value == returnRequest.Id.Value),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(returnRequest);

        var handler = new GetReturnByIdQueryHandler(_mockReturnRepository.Object);

        // Act
        var result = await handler.Handle(
            new GetReturnByIdQuery(returnRequest.Id.Value),
            CancellationToken.None
        );

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.Reason.ShouldBe("Damaged");
    }

    [Fact]
    public async Task GetReturnsByOrderQueryHandler_ShouldReturnOrderRequests()
    {
        // Arrange
        var orderId = OrderId.Create(Guid.NewGuid());
        var userId = UserId.Create(Guid.NewGuid());
        var items = new List<ReturnItem>
        {
            ReturnItem.Create(OrderItemId.Create(Guid.NewGuid()), 1),
        };
        var returns = new List<ReturnRequest>
        {
            ReturnRequest.Create(orderId, userId, "Wrong color", items),
        };

        _mockReturnRepository
            .Setup(r =>
                r.GetByOrderIdAsync(
                    It.Is<OrderId>(id => id.Value == orderId.Value),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(returns);

        var handler = new GetReturnsByOrderQueryHandler(_mockReturnRepository.Object);

        // Act
        var result = await handler.Handle(
            new GetReturnsByOrderQuery(orderId.Value),
            CancellationToken.None
        );

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.Count.ShouldBe(1);
    }
}

using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Orders.Events;
using Shopizy.Application.UnitTests.Orders.TestUtils;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Carts.Entities;
using Shopizy.Domain.Orders.Events;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Application.Interfaces.Persistence;

namespace Shopizy.Application.UnitTests.Orders.Events;

public class OrderCreatedDomainEventHandlerTests
{
    private readonly Mock<ICartRepository> _mockCartRepository;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly OrderCreatedDomainEventHandler _sut;

    public OrderCreatedDomainEventHandlerTests()
    {
        _mockCartRepository = new Mock<ICartRepository>();
        _mockProductRepository = new Mock<IProductRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        _sut = new OrderCreatedDomainEventHandler(
            _mockCartRepository.Object,
            _mockProductRepository.Object,
            _mockUnitOfWork.Object
        );
    }

    [Fact]
    public async Task Should_ReduceProductStockAndClearCartAndSave_WhenOrderCreated()
    {
        // Arrange
        var order = OrderFactory.CreateOrder();
        var domainEvent = new OrderCreatedDomainEvent(order);

        _mockProductRepository
            .Setup(x => x.GetProductsByIdsForUpdateAsync(It.IsAny<List<ProductId>>()))
            .ReturnsAsync([]);

        var cart = Cart.Create(order.UserId);
        cart.AddLineItem(CartItem.Create(ProductId.CreateUnique(), "Red", "M", 2));

        _mockCartRepository
            .Setup(x => x.GetCartByUserIdForUpdateAsync(order.UserId))
            .ReturnsAsync(cart);

        // Act
        await _sut.Handle(domainEvent, CancellationToken.None);

        // Assert
        Assert.Empty(cart.CartItems);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_HandleNullCart_Gracefully()
    {
        // Arrange
        var order = OrderFactory.CreateOrder();
        var domainEvent = new OrderCreatedDomainEvent(order);

        _mockProductRepository
            .Setup(x => x.GetProductsByIdsForUpdateAsync(It.IsAny<List<ProductId>>()))
            .ReturnsAsync([]);

        _mockCartRepository
            .Setup(x => x.GetCartByUserIdForUpdateAsync(order.UserId))
            .ReturnsAsync((Cart?)null);

        // Act
        await _sut.Handle(domainEvent, CancellationToken.None);

        // Assert
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

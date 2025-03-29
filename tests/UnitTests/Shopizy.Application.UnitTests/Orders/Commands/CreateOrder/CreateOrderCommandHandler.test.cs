using FluentAssertions;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Security.CurrentUser;
using Shopizy.Application.Orders.Commands.CreateOrder;
using Shopizy.Application.UnitTests.Orders.TestUtils;
using Shopizy.Application.UnitTests.Products.TestUtils;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Orders;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.UnitTests.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandlerTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<ICurrentUser> _mockCurrentUser;
    private readonly CreateOrderCommandHandler _sut;

    public CreateOrderCommandHandlerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockCurrentUser = new Mock<ICurrentUser>();
        _sut = new CreateOrderCommandHandler(
            _mockProductRepository.Object,
            _mockOrderRepository.Object,
            _mockCurrentUser.Object
        );
    }

    [Fact]
    public async Task Should_HandleValidOrderCreationRequest()
    {
        // Arrange
        var product = ProductFactory.CreateProduct();
        var command = CreateOrderCommandUtils.CreateCommand([product.Id.Value]);

        _mockProductRepository
            .Setup(x => x.GetProductsByIdsAsync(It.IsAny<List<ProductId>>()))
            .ReturnsAsync(() => [product]);

        _mockOrderRepository.Setup(x => x.AddAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
        _mockOrderRepository.Setup(x => x.Commit(CancellationToken.None)).ReturnsAsync(1);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeOfType(typeof(Order));
        result.Value.OrderItems.Count.Should().Be(1);
        result.Value.OrderItems[0].Quantity.Should().Be(command.OrderItems.First().Quantity);

        _mockProductRepository.Verify(
            x => x.GetProductsByIdsAsync(It.IsAny<List<ProductId>>()),
            Times.Once
        );
        _mockOrderRepository.Verify(x => x.AddAsync(It.IsAny<Order>()), Times.Once);
        _mockOrderRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnError_WhenProductIdNotFound()
    {
        // Arrange
        var command = CreateOrderCommandUtils.CreateCommand([]);

        _mockProductRepository
            .Setup(x => x.GetProductsByIdsAsync(It.IsAny<List<ProductId>>()))
            .ReturnsAsync(() => []);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().NotBeNullOrEmpty();
        result.Errors[0].Should().Be(CustomErrors.Product.ProductNotFound);
    }

    // [Fact]
    // public async Task ShouldReturnErrorWhenProductQuantityExceedsStock()
    // {
    //     // Arrange
    //     var productId = ProductId.Create(1);
    //     var orderItem = new CreateOrderCommand.OrderItemDto
    //     {
    //         ProductId = productId.Value,
    //         Quantity = 11, // Exceeds stock quantity
    //     };
    //     var request = new CreateOrderCommand
    //     {
    //         UserId = "user1",
    //         PromoCode = "PROMO10",
    //         DeliveryChargeAmount = 5,
    //         DeliveryChargeCurrency = "USD",
    //         ShippingAddress = new CreateOrderCommand.AddressDto
    //         {
    //             Street = "123 Main St",
    //             City = "New York",
    //             State = "NY",
    //             Country = "USA",
    //             ZipCode = "10001",
    //         },
    //         OrderItems = new List<CreateOrderCommand.OrderItemDto> { orderItem },
    //     };

    //     var product = new Product(
    //         productId,
    //         "Product 1",
    //         "Description",
    //         Price.CreateNew(10, "USD"),
    //         10, // Stock quantity
    //         0
    //     );
    //     _mockProductRepository
    //         .Setup(x =>
    //             x.GetProductsByIdsAsync(It.IsAny<List<ProductId>>(), CancellationToken.None)
    //         )
    //         .ReturnsAsync(new List<Product> { product });

    //     // Act
    //     var result = await _sut.Handle(request, CancellationToken.None);

    //     // Assert
    //     Assert.False(result.IsSuccess);
    //     Assert.Equal(CustomErrors.Product.ProductNotFound, result.Errors[0]);
    // }

    // [Fact]
    // public async Task ShouldHandleValidOrderCreationRequestWithPromoCode()
    // {
    //     // Arrange
    //     var productId = ProductId.Create(1);
    //     var orderItem = new CreateOrderCommand.OrderItemDto
    //     {
    //         ProductId = productId.Value,
    //         Quantity = 2,
    //     };
    //     var request = new CreateOrderCommand
    //     {
    //         UserId = "user1",
    //         PromoCode = "PROMO10", // Valid promo code
    //         DeliveryChargeAmount = 5,
    //         DeliveryChargeCurrency = "USD",
    //         ShippingAddress = new CreateOrderCommand.AddressDto
    //         {
    //             Street = "123 Main St",
    //             City = "New York",
    //             State = "NY",
    //             Country = "USA",
    //             ZipCode = "10001",
    //         },
    //         OrderItems = new List<CreateOrderCommand.OrderItemDto> { orderItem },
    //     };

    //     var product = new Product(
    //         productId,
    //         "Product 1",
    //         "Description",
    //         Price.CreateNew(10, "USD"),
    //         10,
    //         0
    //     );
    //     _mockProductRepository
    //         .Setup(x =>
    //             x.GetProductsByIdsAsync(It.IsAny<List<ProductId>>(), CancellationToken.None)
    //         )
    //         .ReturnsAsync(new List<Product> { product });

    //     _orderRepositoryMock
    //         .Setup(x => x.AddAsync(It.IsAny<Order>(), CancellationToken.None))
    //         .Returns(Task.CompletedTask);
    //     _orderRepositoryMock.Setup(x => x.Commit(CancellationToken.None)).ReturnsAsync(1);

    //     // Act
    //     var result = await _sut.Handle(request, CancellationToken.None);

    //     // Assert
    //     Assert.True(result.IsSuccess);
    //     Assert.Equal(OrderStatus.Pending, result.Data.Status);
    //     Assert.Equal(1, result.Data.OrderItems.Count);
    //     Assert.Equal(orderItem.ProductId, result.Data.OrderItems[0].ProductId.Value);
    //     Assert.Equal(orderItem.Quantity, result.Data.OrderItems[0].Quantity);
    //     Assert.Equal("PROMO10", result.Data.PromoCode); // Verify promo code
    // }

    // [Fact]
    // public async Task ShouldHandleValidOrderCreationRequestWithZeroDeliveryCharge()
    // {
    //     // Arrange
    //     var productId = ProductId.Create(1);
    //     var orderItem = new CreateOrderCommand.OrderItemDto
    //     {
    //         ProductId = productId.Value,
    //         Quantity = 2,
    //     };
    //     var request = new CreateOrderCommand
    //     {
    //         UserId = "user1",
    //         PromoCode = "PROMO10",
    //         DeliveryChargeAmount = 0, // Zero delivery charge
    //         DeliveryChargeCurrency = "USD",
    //         ShippingAddress = new CreateOrderCommand.AddressDto
    //         {
    //             Street = "123 Main St",
    //             City = "New York",
    //             State = "NY",
    //             Country = "USA",
    //             ZipCode = "10001",
    //         },
    //         OrderItems = new List<CreateOrderCommand.OrderItemDto> { orderItem },
    //     };

    //     var product = new Product(
    //         productId,
    //         "Product 1",
    //         "Description",
    //         Price.CreateNew(10, "USD"),
    //         10,
    //         0
    //     );
    //     _mockProductRepository
    //         .Setup(x =>
    //             x.GetProductsByIdsAsync(It.IsAny<List<ProductId>>(), CancellationToken.None)
    //         )
    //         .ReturnsAsync(new List<Product> { product });

    //     _orderRepositoryMock
    //         .Setup(x => x.AddAsync(It.IsAny<Order>(), CancellationToken.None))
    //         .Returns(Task.CompletedTask);
    //     _orderRepositoryMock.Setup(x => x.Commit(CancellationToken.None)).ReturnsAsync(1);

    //     // Act
    //     var result = await _sut.Handle(request, CancellationToken.None);

    //     // Assert
    //     Assert.True(result.IsSuccess);
    //     Assert.Equal(OrderStatus.Pending, result.Data.Status);
    //     Assert.Equal(1, result.Data.OrderItems.Count);
    //     Assert.Equal(orderItem.ProductId, result.Data.OrderItems[0].ProductId.Value);
    //     Assert.Equal(orderItem.Quantity, result.Data.OrderItems[0].Quantity);
    //     Assert.Equal(0, result.Data.DeliveryCharge.Amount); // Verify zero delivery charge
    // }

    // [Fact]
    // public async Task ShouldReturnErrorWhenOrderItemQuantityIsNegative()
    // {
    //     // Arrange
    //     var productId = ProductId.Create(1);
    //     var orderItem = new CreateOrderCommand.OrderItemDto
    //     {
    //         ProductId = productId.Value,
    //         Quantity = -2, // Negative quantity
    //     };
    //     var request = new CreateOrderCommand
    //     {
    //         UserId = "user1",
    //         PromoCode = "PROMO10",
    //         DeliveryChargeAmount = 5,
    //         DeliveryChargeCurrency = "USD",
    //         ShippingAddress = new CreateOrderCommand.AddressDto
    //         {
    //             Street = "123 Main St",
    //             City = "New York",
    //             State = "NY",
    //             Country = "USA",
    //             ZipCode = "10001",
    //         },
    //         OrderItems = new List<CreateOrderCommand.OrderItemDto> { orderItem },
    //     };

    //     var product = new Product(
    //         productId,
    //         "Product 1",
    //         "Description",
    //         Price.CreateNew(10, "USD"),
    //         10,
    //         0
    //     );
    //     _mockProductRepository
    //         .Setup(x =>
    //             x.GetProductsByIdsAsync(It.IsAny<List<ProductId>>(), CancellationToken.None)
    //         )
    //         .ReturnsAsync(new List<Product> { product });

    //     // Act
    //     var result = await _sut.Handle(request, CancellationToken.None);

    //     // Assert
    //     Assert.False(result.IsSuccess);
    //     Assert.Equal(CustomErrors.Order.InvalidOrderItemQuantity, result.Errors.Single());
    // }

    // [Fact]
    // public async Task ShouldHandleValidOrderCreationRequestWithZeroQuantity()
    // {
    //     // Arrange
    //     var productId = ProductId.Create(1);
    //     var orderItem = new CreateOrderCommand.OrderItemDto
    //     {
    //         ProductId = productId.Value,
    //         Quantity = 0, // Zero quantity
    //     };
    //     var request = new CreateOrderCommand
    //     {
    //         UserId = "user1",
    //         PromoCode = "PROMO10",
    //         DeliveryChargeAmount = 5,
    //         DeliveryChargeCurrency = "USD",
    //         ShippingAddress = new CreateOrderCommand.AddressDto
    //         {
    //             Street = "123 Main St",
    //             City = "New York",
    //             State = "NY",
    //             Country = "USA",
    //             ZipCode = "10001",
    //         },
    //         OrderItems = new List<CreateOrderCommand.OrderItemDto> { orderItem },
    //     };

    //     var product = new Product(
    //         productId,
    //         "Product 1",
    //         "Description",
    //         Price.CreateNew(10, "USD"),
    //         10,
    //         0
    //     );
    //     _mockProductRepository
    //         .Setup(x =>
    //             x.GetProductsByIdsAsync(It.IsAny<List<ProductId>>(), CancellationToken.None)
    //         )
    //         .ReturnsAsync(new List<Product> { product });

    //     _orderRepositoryMock
    //         .Setup(x => x.AddAsync(It.IsAny<Order>(), CancellationToken.None))
    //         .Returns(Task.CompletedTask);
    //     _orderRepositoryMock.Setup(x => x.Commit(CancellationToken.None)).ReturnsAsync(1);

    //     // Act
    //     var result = await _sut.Handle(request, CancellationToken.None);

    //     // Assert
    //     Assert.True(result.IsSuccess);
    //     Assert.Equal(OrderStatus.Pending, result.Data.Status);
    //     Assert.Equal(1, result.Data.OrderItems.Count);
    //     Assert.Equal(orderItem.ProductId, result.Data.OrderItems[0].ProductId.Value);
    //     Assert.Equal(orderItem.Quantity, result.Data.OrderItems[0].Quantity);
    // }

    // [Fact]
    // public async Task ShouldHandleValidOrderCreationRequestWithLargeNumberOfOrderItems()
    // {
    //     // Arrange
    //     var productIds = Enumerable.Range(1, 100).Select(ProductId.Create).ToList();
    //     var request = new CreateOrderCommand
    //     {
    //         UserId = "user1",
    //         PromoCode = "PROMO10",
    //         DeliveryChargeAmount = 5,
    //         DeliveryChargeCurrency = "USD",
    //         ShippingAddress = new CreateOrderCommand.AddressDto
    //         {
    //             Street = "123 Main St",
    //             City = "New York",
    //             State = "NY",
    //             Country = "USA",
    //             ZipCode = "10001",
    //         },
    //         OrderItems = productIds
    //             .Select(id => new CreateOrderCommand.OrderItemDto
    //             {
    //                 ProductId = id.Value,
    //                 Quantity = 2,
    //             })
    //             .ToList(),
    //     };

    //     var products = productIds
    //         .Select(id => new Product(
    //             id,
    //             $"Product {id.Value}",
    //             "Description",
    //             Price.CreateNew(10, "USD"),
    //             10,
    //             0
    //         ))
    //         .ToList();
    //     _mockProductRepository
    //         .Setup(x =>
    //             x.GetProductsByIdsAsync(It.IsAny<List<ProductId>>(), CancellationToken.None)
    //         )
    //         .ReturnsAsync(products);

    //     _orderRepositoryMock
    //         .Setup(x => x.AddAsync(It.IsAny<Order>(), CancellationToken.None))
    //         .Returns(Task.CompletedTask);
    //     _orderRepositoryMock.Setup(x => x.Commit(CancellationToken.None)).ReturnsAsync(1);

    //     // Act
    //     var result = await _sut.Handle(request, CancellationToken.None);

    //     // Assert
    //     Assert.True(result.IsSuccess);
    //     Assert.Equal(OrderStatus.Pending, result.Data.Status);
    //     Assert.Equal(100, result.Data.OrderItems.Count);
    //     Assert.All(
    //         result.Data.OrderItems,
    //         item =>
    //         {
    //             var requestItem = request.OrderItems.First(x =>
    //                 x.ProductId == item.ProductId.Value
    //             );
    //             Assert.Equal(requestItem.ProductId, item.ProductId.Value);
    //             Assert.Equal(requestItem.Quantity, item.Quantity);
    //         }
    //     );
    // }

    // [Fact]
    // public async Task ShouldHandleValidOrderCreationRequestWithLargeDeliveryCharge()
    // {
    //     // Arrange
    //     var productId = ProductId.Create(1);
    //     var orderItem = new CreateOrderCommand.OrderItemDto
    //     {
    //         ProductId = productId.Value,
    //         Quantity = 2,
    //     };
    //     var request = new CreateOrderCommand
    //     {
    //         UserId = "user1",
    //         PromoCode = "PROMO10",
    //         DeliveryChargeAmount = 1000, // Large delivery charge amount
    //         DeliveryChargeCurrency = "USD",
    //         ShippingAddress = new CreateOrderCommand.AddressDto
    //         {
    //             Street = "123 Main St",
    //             City = "New York",
    //             State = "NY",
    //             Country = "USA",
    //             ZipCode = "10001",
    //         },
    //         OrderItems = new List<CreateOrderCommand.OrderItemDto> { orderItem },
    //     };

    //     var product = new Product(
    //         productId,
    //         "Product 1",
    //         "Description",
    //         Price.CreateNew(10, "USD"),
    //         10,
    //         0
    //     );
    //     _mockProductRepository
    //         .Setup(x =>
    //             x.GetProductsByIdsAsync(It.IsAny<List<ProductId>>(), CancellationToken.None)
    //         )
    //         .ReturnsAsync(new List<Product> { product });

    //     _orderRepositoryMock
    //         .Setup(x => x.AddAsync(It.IsAny<Order>(), CancellationToken.None))
    //         .Returns(Task.CompletedTask);
    //     _orderRepositoryMock.Setup(x => x.Commit(CancellationToken.None)).ReturnsAsync(1);

    //     // Act
    //     var result = await _sut.Handle(request, CancellationToken.None);

    //     // Assert
    //     Assert.True(result.IsSuccess);
    //     Assert.Equal(OrderStatus.Pending, result.Data.Status);
    //     Assert.Equal(1, result.Data.OrderItems.Count);
    //     Assert.Equal(orderItem.ProductId, result.Data.OrderItems[0].ProductId.Value);
    //     Assert.Equal(orderItem.Quantity, result.Data.OrderItems[0].Quantity);
    // }

    // [Fact]
    // public async Task ShouldHandleValidOrderCreationRequestWithNonExistentUserId()
    // {
    //     // Arrange
    //     var productId = ProductId.Create(1);
    //     var orderItem = new CreateOrderCommand.OrderItemDto
    //     {
    //         ProductId = productId.Value,
    //         Quantity = 2,
    //     };
    //     var request = new CreateOrderCommand
    //     {
    //         UserId = "non_existent_user", // Non-existent user ID
    //         PromoCode = "PROMO10",
    //         DeliveryChargeAmount = 5,
    //         DeliveryChargeCurrency = "USD",
    //         ShippingAddress = new CreateOrderCommand.AddressDto
    //         {
    //             Street = "123 Main St",
    //             City = "New York",
    //             State = "NY",
    //             Country = "USA",
    //             ZipCode = "10001",
    //         },
    //         OrderItems = new List<CreateOrderCommand.OrderItemDto> { orderItem },
    //     };

    //     var product = new Product(
    //         productId,
    //         "Product 1",
    //         "Description",
    //         Price.CreateNew(10, "USD"),
    //         10,
    //         0
    //     );
    //     _mockProductRepository
    //         .Setup(x =>
    //             x.GetProductsByIdsAsync(It.IsAny<List<ProductId>>(), CancellationToken.None)
    //         )
    //         .ReturnsAsync(new List<Product> { product });

    //     _orderRepositoryMock
    //         .Setup(x => x.AddAsync(It.IsAny<Order>(), CancellationToken.None))
    //         .Returns(Task.CompletedTask);
    //     _orderRepositoryMock.Setup(x => x.Commit(CancellationToken.None)).ReturnsAsync(1);

    //     // Act
    //     var result = await _sut.Handle(request, CancellationToken.None);

    //     // Assert
    //     Assert.True(result.IsSuccess);
    //     Assert.Equal(OrderStatus.Pending, result.Data.Status);
    //     Assert.Equal(1, result.Data.OrderItems.Count);
    //     Assert.Equal(orderItem.ProductId, result.Data.OrderItems[0].ProductId.Value);
    //     Assert.Equal(orderItem.Quantity, result.Data.OrderItems[0].Quantity);
    // }
}

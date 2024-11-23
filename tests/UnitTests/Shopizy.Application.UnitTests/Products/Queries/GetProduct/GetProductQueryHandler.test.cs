using FluentAssertions;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Products.Queries.GetProduct;
using Shopizy.Application.UnitTests.Products.TestUtils;
using Shopizy.Application.UnitTests.TestUtils.Extensions;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.UnitTests.Products.Queries.GetProduct;

public class GetProductQueryHandlerTests
{
    private readonly GetProductQueryHandler _sut;
    private readonly Mock<IProductRepository> _mockProductRepository;

    public GetProductQueryHandlerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _sut = new GetProductQueryHandler(_mockProductRepository.Object);
    }

    // [Fact]
    // public async Task ShouldReturnNullWhenProductIdDoesNotExistInRepository()
    // {
    //     // Arrange
    //     var mockProductRepository = new Mock<IProductRepository>();
    //     var productId = ProductId.Create(1);
    //     mockProductRepository
    //         .Setup(repo => repo.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
    //         .ReturnsAsync((Product?)null);

    //     var queryHandler = new GetProductQueryHandler(mockProductRepository.Object);
    //     var query = new GetProductQuery { ProductId = productId.Value };

    //     // Act
    //     var result = await queryHandler.Handle(query, CancellationToken.None);

    //     // Assert
    //     Assert.IsType<Response<Product?>>(result);
    //     Assert.Null(result.Value);
    // }

    // [Fact]
    // public async Task ShouldThrowExceptionWhenProductIdIsNull()
    // {
    //     // Arrange
    //     var mockProductRepository = new Mock<IProductRepository>();
    //     ProductId? productId = null;

    //     var queryHandler = new GetProductQueryHandler(mockProductRepository.Object);
    //     var query = new GetProductQuery { ProductId = productId };

    //     // Act & Assert
    //     await Assert.ThrowsAsync<ArgumentNullException>(
    //         async () => await queryHandler.Handle(query, CancellationToken.None)
    //     );
    // }

    [Fact]
    public async Task ShouldReturnCorrectProductWhenProductIdExistsInRepositoryAsync()
    {
        // Arrange
        var product = ProductFactory.CreateProduct();
        var query = GetProductQueryUtils.CreateQuery();
        _mockProductRepository
            .Setup(c => c.GetProductByIdAsync(ProductId.Create(query.ProductId)))
            .ReturnsAsync(product);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeOfType<Product>();
        result.Value.ValidateResult(query);
    }

    // [Fact]
    // public async Task ShouldHandleConcurrentRequestsWithoutDataCorruption()
    // {
    //     // Arrange
    //     var mockProductRepository = new Mock<IProductRepository>();
    //     var productId = ProductId.Create(1);
    //     var product = new Product(productId, "Test Product", "Test Description", 100m);
    //     mockProductRepository
    //         .Setup(repo => repo.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(product);

    //     var queryHandler = new GetProductQueryHandler(mockProductRepository.Object);
    //     var query = new GetProductQuery { ProductId = productId.Value };

    //     var tasks = new List<Task<ErrorOr<Product?>>>();
    //     for (int i = 0; i < 10; i++)
    //     {
    //         tasks.Add(queryHandler.Handle(query, CancellationToken.None));
    //     }

    //     // Act
    //     await Task.WhenAll(tasks);

    //     // Assert
    //     foreach (var task in tasks)
    //     {
    //         Assert.IsType<Response<Product?>>(task.Result);
    //         Assert.Equal(product, task.Result.Value);
    //     }
    // }

    // [Fact]
    // public async Task ShouldReturnErrorResponseWhenDatabaseConnectionIsLost()
    // {
    //     // Arrange
    //     var mockProductRepository = new Mock<IProductRepository>();
    //     var productId = ProductId.Create(1);
    //     mockProductRepository
    //         .Setup(repo => repo.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
    //         .ThrowsAsync(new Exception("Database connection lost"));

    //     var queryHandler = new GetProductQueryHandler(mockProductRepository.Object);
    //     var query = new GetProductQuery { ProductId = productId.Value };

    //     // Act
    //     var result = await queryHandler.Handle(query, CancellationToken.None);

    //     // Assert
    //     Assert.IsType<Response<Product?>>(result);
    //     Assert.False(result.Success);
    //     Assert.Equal("Database connection lost", result.Error);
    // }

    // [Fact]
    // public async Task ShouldReturnErrorResponseWhenProductIdIsNotValidGuid()
    // {
    //     // Arrange
    //     var mockProductRepository = new Mock<IProductRepository>();
    //     var invalidProductId = "not-a-valid-guid";
    //     mockProductRepository
    //         .Setup(repo =>
    //             repo.GetProductByIdAsync(It.IsAny<ProductId>(), It.IsAny<CancellationToken>())
    //         )
    //         .ReturnsAsync((Product?)null);

    //     var queryHandler = new GetProductQueryHandler(mockProductRepository.Object);
    //     var query = new GetProductQuery { ProductId = invalidProductId };

    //     // Act
    //     var result = await queryHandler.Handle(query, CancellationToken.None);

    //     // Assert
    //     Assert.IsType<Response<Product?>>(result);
    //     Assert.False(result.Success);
    //     Assert.Equal("Invalid product ID.", result.Message);
    // }

    // [Fact]
    // public async Task ShouldReturnErrorResponseWhenProductRepositoryThrowsException()
    // {
    //     // Arrange
    //     var mockProductRepository = new Mock<IProductRepository>();
    //     var productId = ProductId.Create(1);
    //     var exception = new Exception("An error occurred while fetching product");
    //     mockProductRepository
    //         .Setup(repo => repo.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
    //         .ThrowsAsync(exception);

    //     var queryHandler = new GetProductQueryHandler(mockProductRepository.Object);
    //     var query = new GetProductQuery { ProductId = productId.Value };

    //     // Act
    //     var result = await queryHandler.Handle(query, CancellationToken.None);

    //     // Assert
    //     Assert.IsType<Response<Product?>>(result);
    //     Assert.False(result.IsSuccess);
    //     Assert.Equal(exception.Message, result.Error);
    // }

    // [Fact]
    // public async Task ShouldReturnSuccessResponseWithNullWhenProductIdIsNotProvided()
    // {
    //     // Arrange
    //     var mockProductRepository = new Mock<IProductRepository>();
    //     mockProductRepository
    //         .Setup(repo =>
    //             repo.GetProductByIdAsync(It.IsAny<ProductId>(), It.IsAny<CancellationToken>())
    //         )
    //         .ReturnsAsync((Product?)null);

    //     var queryHandler = new GetProductQueryHandler(mockProductRepository.Object);
    //     var query = new GetProductQuery { ProductId = 0 };

    //     // Act
    //     var result = await queryHandler.Handle(query, CancellationToken.None);

    //     // Assert
    //     Assert.IsType<Response<Product?>>(result);
    //     Assert.True(result.IsSuccess);
    //     Assert.Null(result.Value);
    // }

    // [Fact]
    // public async Task ShouldReturnSuccessResponseWithNullWhenProductIsNotFoundInRepository()
    // {
    //     // Arrange
    //     var mockProductRepository = new Mock<IProductRepository>();
    //     var productId = ProductId.Create(1);
    //     mockProductRepository
    //         .Setup(repo => repo.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
    //         .ReturnsAsync((Product?)null);

    //     var queryHandler = new GetProductQueryHandler(mockProductRepository.Object);
    //     var query = new GetProductQuery { ProductId = productId.Value };

    //     // Act
    //     var result = await queryHandler.Handle(query, CancellationToken.None);

    //     // Assert
    //     Assert.IsType<Response<Product?>>(result);
    //     Assert.True(result.IsSuccess);
    //     Assert.Null(result.Value);
    // }

    // [Fact]
    // public async Task ShouldHandleALargeNumberOfRequestsWithoutSignificantPerformanceDegradation()
    // {
    //     // Arrange
    //     var mockProductRepository = new Mock<IProductRepository>();
    //     var productId = ProductId.Create(1);
    //     mockProductRepository
    //         .Setup(repo => repo.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(new Product(productId, "Test Product", "Test Description", 100m));

    //     var queryHandler = new GetProductQueryHandler(mockProductRepository.Object);
    //     var query = new GetProductQuery { ProductId = productId.Value };

    //     // Act
    //     var tasks = Enumerable
    //         .Range(1, 1000)
    //         .Select(_ => queryHandler.Handle(query, CancellationToken.None));

    //     await Task.WhenAll(tasks);

    //     // Assert
    //     mockProductRepository.Verify(
    //         repo => repo.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()),
    //         Times.Exactly(1000)
    //     );
    // }
}

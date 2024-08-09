using FluentAssertions;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Products.Queries.GetProduct;
using Shopizy.Application.UnitTests.Products.TestUtils;
using Shopizy.Application.UnitTests.TestUtils.Extensions;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.UnitTests.Products.Queries.GetProduct;

public class GetProductQueryHandlerTests
{
    private readonly GetProductQueryHandler _handler;
    private readonly Mock<IProductRepository> _mockProductRepository;

    public GetProductQueryHandlerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _handler = new GetProductQueryHandler(_mockProductRepository.Object);
    }

    [Fact]
    public async Task GetProduct_WhenProductIsFound_ReturnProduct()
    {
        // Arrange
        var product = ProductFactory.CreateProduct();
        var query = GetProductQueryUtils.CreateQuery();
        _mockProductRepository
            .Setup(c => c.GetProductByIdAsync(ProductId.Create(query.ProductId)))
            .ReturnsAsync(product);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value?.ValidateResult(query);
    }

    // [Fact]
    // public async Task Handle_GivenValidRequestReturnProducts()
    // {
    //     // Arrange
    //     var products = new List<Product>()
    //     {
    //         new Product("Product 1", 10),
    //         new Product("Product 2", 20),
    //         new Product("Product 3", 30)
    //     };
    //     var productRepositoryMock = new Mock<IProductRepository>();
    //     productRepositoryMock.Setup(x => x.GetProductsAsync()).ReturnsAsync(products);

    //     var handler = new ListProductQueryHandler(productRepositoryMock.Object);

    //     // Act
    //     var result = await handler.Handle(new ListProductQuery(), CancellationToken.None);

    //     // Assert
    //     result.Match(
    //         products =>
    //         {
    //             Assert.Equal(products, result);
    //             return true;
    //         },
    //         errors => Assert.True(false, $"An error occurred: {string.Join(", ", errors.Select(x => x.Message))}"));
    // }
}

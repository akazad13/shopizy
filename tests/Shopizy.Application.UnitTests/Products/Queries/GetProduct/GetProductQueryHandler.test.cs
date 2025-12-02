using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Products.Queries.GetProduct;
using Shopizy.Application.UnitTests.Products.TestUtils;
using Shopizy.Application.UnitTests.TestUtils.Extensions;
using Shopizy.Domain.Common.CustomErrors;
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

    [Fact]
    public async Task Should_ReturnCorrectProduct_WhenProductIdExistsInRepository()
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
        Assert.False(result.IsError);
        Assert.NotNull(result.Value);
        Assert.IsType<Product>(result.Value);
        result.Value.ValidateResult(query);
    }

    [Fact]
    public async Task Should_ReturnProductNotFound_When_ProductDoesNotExist()
    {
        // Arrange
        var query = GetProductQueryUtils.CreateQuery();
        _mockProductRepository
            .Setup(c => c.GetProductByIdAsync(ProductId.Create(query.ProductId)))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(CustomErrors.Product.ProductNotFound, result.FirstError);
    }
}

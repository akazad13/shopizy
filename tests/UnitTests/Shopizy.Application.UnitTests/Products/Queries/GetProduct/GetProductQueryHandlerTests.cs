using FluentAssertions;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Application.Products.Queries.GetProduct;
using Shopizy.Application.UnitTests.Products.Queries.TestUtils;
using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Application.UnitTests.TestUtils.Products.Extensions;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Products;
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
    public async void GetProduct_WhenProductIsFound_ShouldReturnProduct()
    {
        // Arrange
        var getProductQuery = GetProductQueryUtils.CreateQuery();
        _mockProductRepository
            .Setup(c => c.GetProductByIdAsync(ProductId.Create(getProductQuery.ProductId)))
            .ReturnsAsync(
                Product.Create(
                    Constants.Product.Name,
                    Constants.Product.Description,
                    Constants.Category.Id,
                    Constants.Product.Sku,
                    Constants.Product.StockQuantity,
                    Price.CreateNew(Constants.Product.UnitPrice, Constants.Product.Currency),
                    Constants.Product.Discount,
                    Constants.Product.Brand,
                    Constants.Product.Barcode,
                    Constants.Product.Tags,
                    ""
                )
            );

        // Act
        var result = await _handler.Handle(getProductQuery, default);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value?.ValidateCreatedForm(getProductQuery);
    }

    // [Fact]
    // public async Task Handle_GivenValidRequest_ShouldReturnProducts()
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

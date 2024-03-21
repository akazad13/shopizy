using FluentAssertions;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Application.Products.Queries.GetProduct;
using Shopizy.Application.UnitTests.Products.Queries.TestUtils;
using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Application.UnitTests.TestUtils.Products.Extensions;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Products;

namespace Shopizy.Application.UnitTests.Products.Queries.GetProduct;

public class GetProductQueryHandlerTest
{
    private readonly GetProductQueryHandler _handler;
    private readonly Mock<IProductRepository> _mockProductRepository;

    public GetProductQueryHandlerTest()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _handler = new GetProductQueryHandler(_mockProductRepository.Object);
    }

    [Fact]
    public async void GetProductQuery_WhenProductIsFound_ShouldReturnProduct()
    {
        var getProductQuery = GetProductQueryUtils.CreateQuery();
        _mockProductRepository
            .Setup(c => c.GetProductByIdAsync(getProductQuery.ProductId))
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

        var result = await _handler.Handle(getProductQuery, default);

        result.IsError.Should().BeFalse();
        result.Value?.ValidateCreatedForm(getProductQuery);
    }
}

using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Products.Commands.UpdateProduct;
using Shopizy.Application.UnitTests.Products.TestUtils;
using Shopizy.Domain.Products;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.UnitTests.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandlerTests
{
    private readonly UpdateProductCommandHandler _sut;
    private readonly Mock<IProductRepository> _mockProductRepository;

    public UpdateProductCommandHandlerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _sut = new UpdateProductCommandHandler(_mockProductRepository.Object);
    }

    [Fact]
    public async Task Should_UpdateTheProductSuccessfully_WhenAllRequiredFieldsAreProvided()
    {
        // Arrange
        var command = UpdateProductCommandUtils.CreateCommand();
        var product = ProductFactory.CreateProduct();

        _mockProductRepository
            .Setup(x => x.GetProductByIdAsync(It.IsAny<ProductId>()))
            .ReturnsAsync(product);

        _mockProductRepository.Setup(x => x.Update(It.IsAny<Product>()));
        _mockProductRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.IsType<Success>(result.Value);

        _mockProductRepository.Verify(
            x => x.GetProductByIdAsync(It.IsAny<ProductId>()),
            Times.Once
        );
        _mockProductRepository.Verify(x => x.Update(It.IsAny<Product>()), Times.Once);
        _mockProductRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    // [Fact]
    // public async Task Should_ReturnError_WhenUpdatingProductWithInvalidProductId()
    // {
    //     // Implementation
    // }

    // [Fact]
    // public async Task Should_ReturnError_WhenUpdatingProductWithNonExistingCategoryId()
    // {
    //     // Implementation
    // }

    // [Fact]
    // public async Task Should_HandleConcurrentUpdates_ToTheSameProductWithoutDataCorruption()
    // {
    //     // Implementation
    // }

    // [Fact]
    // public async Task Should_ReturnError_WhenUpdatingProductWithNegativeUnitPrice()
    // {
    //     // Implementation
    // }

    // [Fact]
    // public async Task Should_ReturnError_WhenUpdatingProductWithInvalidCurrencyCode()
    // {
    //     // Implementation
    // }

    // [Fact]
    // public async Task Should_ReturnError_WhenUpdatingProductWithDiscountGreaterThan100Percent()
    // {
    //     // Implementation
    // }

    // [Fact]
    // public async Task Should_ReturnError_WhenProductRepositoryFailsToCommitChanges()
    // {
    //     // Implementation
    // }

    // [Fact]
    // public async Task Should_ReturnTheUpdatedProduct_WhenTheUpdateIsSuccessful()
    // {
    //     // Implementation
    // }

    // [Fact]
    // public void Should_ValidateAndSanitizeInput_ToPreventSecurityVulnerabilities()
    // {
    //     // Implementation
    // }
}

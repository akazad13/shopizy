using ErrorOr;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Products.Commands.CreateProduct;
using Shopizy.Application.UnitTests.Products.TestUtils;
using Shopizy.Application.UnitTests.TestUtils.Extensions;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Products;

namespace Shopizy.Application.UnitTests.Products.Commands.CreateProduct;

public class CreateProductCommandHandlerTests
{
    // T1: SUT (system under testing) - logical component we are testing
    // T2: Scenario - What we are testing
    // T3: Expected Outcome - what we expect the logical component to do

    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly CreateProductCommandHandler _sut;

    public CreateProductCommandHandlerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _sut = new CreateProductCommandHandler(_mockProductRepository.Object);
    }

    [Fact]
    public async Task Should_ThrowException_WhenProductNameIsEmpty()
    {
        // Arrange
        var command = CreateProductCommandUtils.CreateCommandWithEmptyProductName();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.NotNull(result.Errors);
        Assert.Equal(CustomErrors.Product.ProductNotCreated, result.Errors[0]);

        _mockProductRepository.Verify(m => m.AddAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task Should_CreateAndReturnProduct_WhenProductIsValid()
    {
        // Arrange
        var command = CreateProductCommandUtils.CreateCommand();

        _mockProductRepository.Setup(p => p.AddAsync(It.IsAny<Product>()));

        // Act
        var result = await _sut.Handle(command, default);

        // Assert
        Assert.False(result.IsError);
        Assert.IsType<Product>(result.Value);
        Assert.NotNull(result.Value);
        result.Value.ValidateResult(command);

        _mockProductRepository.Verify(m => m.AddAsync(It.IsAny<Product>()), Times.Once);
    }
}

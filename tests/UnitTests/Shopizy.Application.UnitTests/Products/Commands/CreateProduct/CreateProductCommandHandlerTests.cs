using FluentAssertions;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Products.Commands.CreateProduct;
using Shopizy.Application.UnitTests.Products.TestUtils;
using Shopizy.Application.UnitTests.TestUtils.Extensions;

namespace Shopizy.Application.UnitTests.Products.Commands.CreateProduct;

public class CreateProductCommandHandlerTests
{
    // T1: SUT (system under testing) - logical component we are testing
    // T2: Scenerio - What we are testing
    // T3: Expected Outcome - what we expect the logical component to do

    private readonly CreateProductCommandHandler _handler;
    private readonly Mock<IProductRepository> _mockProductRepository;

    public CreateProductCommandHandlerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _handler = new CreateProductCommandHandler(_mockProductRepository.Object);
    }

    [Fact]
    public async Task CreateProduct_WhenProductIsValid_ShouldCreateAndReturnProduct()
    {
        // Arrange

        var command = CreateProductCommandUtils.CreateCommand();
        var product = ProductFactory.CreateProduct();

        _mockProductRepository.Setup(p => p.AddAsync(product));
        _mockProductRepository.Setup(p => p.Commit(default)).ReturnsAsync(1);
        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.ValidateResult(command);

        _mockProductRepository.Verify(m => m.AddAsync(result.Value), Times.Once);
        _mockProductRepository.Verify(m => m.Commit(default), Times.Once);
    }

    // [Theory]
    // [MemberData(nameof(ValidCreateProductCommands))]
    // public async void CreateProductCommand_WhenProductIsValid_ShouldCreateAndReturnProduct1(
    //     CreateProductCommand createProductCommand
    // )
    // {
    //     var product = Product.Create(
    //         createProductCommand.Name,
    //         createProductCommand.Description,
    //         createProductCommand.CategoryId,
    //         createProductCommand.Sku,
    //         createProductCommand.StockQuantity,
    //         Price.CreateNew(createProductCommand.UnitPrice, createProductCommand.Currency),
    //         createProductCommand.Discount,
    //         createProductCommand.Brand,
    //         createProductCommand.Barcode,
    //         createProductCommand.Tags,
    //         ""
    //     );

    //     _mockProductRepository.Setup(p => p.AddAsync(product));
    //     _mockProductRepository.Setup(p => p.Commit(default)).ReturnsAsync(1);
    //     //Act
    //     var result = await _handler.Handle(createProductCommand, default);

    //     //Assert
    //     result.IsError.Should().BeFalse();
    //     result.Value.ValidateCreatedForm(createProductCommand);

    //     _mockProductRepository.Verify(m => m.AddAsync(result.Value), Times.Once);
    // }

    // public static IEnumerable<object[]> ValidCreateProductCommands()
    // {
    //     yield return new[] { CreateProductCommandUtils.CreateCommand() };
    // }
}

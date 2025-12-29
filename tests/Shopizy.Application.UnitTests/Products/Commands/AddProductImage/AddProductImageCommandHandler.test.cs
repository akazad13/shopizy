using Microsoft.AspNetCore.Http;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Products.Commands.AddProductImage;
using Shopizy.Application.Products.Common;
using Shopizy.Application.UnitTests.Products.TestUtils;
using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Products.Entities;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.UnitTests.Products.Commands.AddProductImage;

public class AddProductImageCommandHandlerTests
{
    private readonly AddProductImageCommandHandler _sut;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<IMediaUploader> _mockMediaUploader;

    public AddProductImageCommandHandlerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockMediaUploader = new Mock<IMediaUploader>();
        _sut = new AddProductImageCommandHandler(
            _mockProductRepository.Object,
            _mockMediaUploader.Object
        );
    }

    [Fact]
    public async Task Should_ThrowException_WhenFileIsNull()
    {
        // Arrange
        var command = new AddProductImageCommand(Constants.User.Id.Value, Constants.Product.Id.Value, null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.NotNull(result.Errors);
        Assert.Equal(CustomErrors.Product.ProductImageNotUploaded, result.Errors[0]);
    }

    [Fact]
    public async Task Should_ReturnErrorResponse_WhenProductIsNotFound()
    {
        // Arrange
        var command = AddProductImageCommandUtils.CreateCommand(Constants.User.Id.Value, Constants.Product.Id.Value);

        _mockProductRepository
            .Setup(x => x.GetProductByIdAsync(It.IsAny<ProductId>()))
            .ReturnsAsync(() => null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.NotNull(result.Errors);
        Assert.Equal(CustomErrors.Product.ProductNotFound, result.Errors[0]);
    }

    [Fact]
    public async Task Should_ReturnProductImage_WhenProductIsFoundAndImageIsUploaded()
    {
        // Arrange
        var product = ProductFactory.CreateProduct();
        var productImage = ProductFactory.CreateProductImage();
        product.AddProductImage(productImage);

        var command = AddProductImageCommandUtils.CreateCommand(Constants.User.Id.Value, product.Id.Value);

        _mockProductRepository
            .Setup(p => p.GetProductByIdAsync(ProductId.Create(command.ProductId)))
            .ReturnsAsync(product);

        _mockMediaUploader
            .Setup(cl => cl.UploadPhotoAsync(It.IsAny<IFormFile>(), default))
            .ReturnsAsync(
                () =>
                    new PhotoUploadResult(
                        Constants.ProductImage.ImageUrl,
                        Constants.ProductImage.PublicId
                    )
            );

        _mockProductRepository.Setup(p => p.Update(product));
        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Value);
        Assert.IsType<ProductImage>(result.Value);
        Assert.Equal(productImage.ImageUrl, result.Value.ImageUrl);
        Assert.Equal(productImage.PublicId, result.Value.PublicId);

    }
}

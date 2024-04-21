using FluentAssertions;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Products.Commands.AddProductImage;
using Shopizy.Application.UnitTests.Products.TestUtils;
using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Application.Products.Common;
using Microsoft.AspNetCore.Http;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Products.Entities;

namespace Shopizy.Application.UnitTests.Products.Commands.AddProductImage;

public class AddProductImageCommandHandlerTests
{
    private readonly AddProductImageCommandHandler _handler;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<IMediaUploader> _mockMediaUploader;

    public AddProductImageCommandHandlerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockMediaUploader = new Mock<IMediaUploader>();
        _handler = new AddProductImageCommandHandler(
            _mockProductRepository.Object,
            _mockMediaUploader.Object
        );
    }

    [Fact]
    public async Task AddProductImage_WhenProductIsFoundAndImageIsUploaded_ShouldAddAndReturnProductImage()
    {
        // Arrange
        var product = ProductFactory.CreateProduct();
        var productImage = ProductFactory.CreateProductImage();
        product.AddProductImage(productImage);

        var command = AddProductImageCommandUtils.CreateCommand(product.Id.Value);

        _mockProductRepository
            .Setup(p => p.GetProductByIdAsync(ProductId.Create(command.ProductId)))
            .ReturnsAsync(product);

        _mockMediaUploader
            .Setup(cl => cl.UploadPhotoAsync(It.IsAny<IFormFile>(), default))
            .ReturnsAsync(
                new PhotoUploadResult(
                    Constants.ProductImage.ImageUrl,
                    Constants.ProductImage.PublicId
                )
            );

        _mockProductRepository.Setup(p => p.Update(product));
        _mockProductRepository.Setup(p => p.Commit(default)).ReturnsAsync(1);
        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType(typeof(ProductImage));
        result.Value.ImageUrl.Should().Be(productImage.ImageUrl);
        result.Value.PublicId.Should().Be(productImage.PublicId);

        _mockProductRepository.Verify(m => m.Commit(default), Times.Once);
    }
}

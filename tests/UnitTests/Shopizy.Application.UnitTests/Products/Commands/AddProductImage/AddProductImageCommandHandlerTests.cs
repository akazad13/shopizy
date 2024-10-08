using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Products.Commands.AddProductImage;
using Shopizy.Application.Products.Common;
using Shopizy.Application.UnitTests.Products.TestUtils;
using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Domain.Products.Entities;
using Shopizy.Domain.Products.ValueObjects;

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
    public async Task AddProductImage_WhenProductIsFoundAndImageIsUploaded_AddAndReturnProductImage()
    {
        // Arrange
        Domain.Products.Product product = ProductFactory.CreateProduct();
        ProductImage productImage = ProductFactory.CreateProductImage();
        product.AddProductImage(productImage);

        AddProductImageCommand command = AddProductImageCommandUtils.CreateCommand(product.Id.Value);

        _ = _mockProductRepository
            .Setup(p => p.GetProductByIdAsync(ProductId.Create(command.ProductId)))
            .ReturnsAsync(product);

        _ = _mockMediaUploader
            .Setup(cl => cl.UploadPhotoAsync(It.IsAny<IFormFile>(), default))
            .ReturnsAsync(
                new PhotoUploadResult(
                    Constants.ProductImage.ImageUrl,
                    Constants.ProductImage.PublicId
                )
            );

        _ = _mockProductRepository.Setup(p => p.Update(product));
        _ = _mockProductRepository.Setup(p => p.Commit(default)).ReturnsAsync(1);
        // Act
        ErrorOr.ErrorOr<ProductImage> result = await _handler.Handle(command, default);

        // Assert
        _ = result.IsError.Should().BeFalse();
        _ = result.Value.Should().BeOfType(typeof(ProductImage));
        _ = result.Value.ImageUrl.Should().Be(productImage.ImageUrl);
        _ = result.Value.PublicId.Should().Be(productImage.PublicId);

        _mockProductRepository.Verify(m => m.Commit(default), Times.Once);
    }
}

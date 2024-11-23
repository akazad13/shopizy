using FluentAssertions;
using Moq;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Products.Commands.DeleteProductImage;
using Shopizy.Application.UnitTests.Products.TestUtils;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.UnitTests.Products.Commands.DeleteProductImage;

public class DeleteProductImageCommandHandlerTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<IMediaUploader> _mockMediaUploader;
    private readonly DeleteProductImageCommandHandler _sut;

    public DeleteProductImageCommandHandlerTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockMediaUploader = new Mock<IMediaUploader>();
        _sut = new DeleteProductImageCommandHandler(
            _mockProductRepository.Object,
            _mockMediaUploader.Object
        );
    }

    [Fact]
    public async Task ShouldDeleteAndReturnSuccessWhenProductIsFoundAndImageIsFoundAsync()
    {
        // Arrange
        var product = ProductFactory.CreateProduct();
        var productImage = ProductFactory.CreateProductImage();
        product.AddProductImage(productImage);

        var command = DeleteProductImageCommandUtils.CreateCommand(
            product.Id.Value,
            productImage.Id.Value
        );

        _mockProductRepository
            .Setup(p => p.GetProductByIdAsync(ProductId.Create(command.ProductId)))
            .ReturnsAsync(product);

        _mockMediaUploader
            .Setup(cl => cl.DeletePhotoAsync(productImage.PublicId))
            .ReturnsAsync(Result.Success);

        _mockProductRepository.Setup(p => p.Update(product));
        _mockProductRepository.Setup(p => p.Commit(default)).ReturnsAsync(1);
        // Act
        var result = (await _sut.Handle(command, default)).Match(x => x, x => null);

        // Assert
        result.Should().BeOfType<Success>();
        result.Should().NotBeNull();
        result.Message.Should().Be("Delete product image successfully.");
        result.Errors.Should().BeEmpty();

        _mockProductRepository.Verify(m => m.Commit(default), Times.Once);
    }

    // [Fact]
    // public async Task ShouldHandleARequestWithValidProductAndImageIDs()
    // {
    //     // Arrange
    //     var productId = ProductId.Create("valid-product-id");
    //     var imageId = ProductImageId.Create("valid-image-id");
    //     var command = new DeleteProductImageCommand(productId.Value, imageId.Value);

    //     var product = new Product(productId, "Test Product");
    //     product.AddProductImage(new ProductImage(ProductImageId.Create("image1"), "publicId1"));
    //     product.AddProductImage(new ProductImage(imageId, "publicId2"));

    //     _mockProductRepository
    //         .Setup(x => x.GetProductByIdAsync(productId, CancellationToken.None))
    //         .ReturnsAsync(product);

    //     _mockMediaUploader
    //         .Setup(x => x.DeletePhotoAsync("publicId2", CancellationToken.None))
    //         .ReturnsAsync(Result<Success>.SuccessResponese("Success"));

    //     _mockProductRepository.Setup(x => x.Update(product));

    //     _mockProductRepository.Setup(x => x.Commit(CancellationToken.None)).ReturnsAsync(1);

    //     // Act
    //     var result = await _sut.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.True(result.Succeeded);
    //     Assert.Equal("Delete product image successfully.", result.Data.Message);
    // }

    // [Fact]
    // public async Task ShouldReturnErrorResponseWhenProductIDIsNotFound()
    // {
    //     // Arrange
    //     var productId = "non-existent-product-id";
    //     var imageId = "valid-image-id";
    //     var command = new DeleteProductImageCommand(productId, imageId);

    //     _mockProductRepository
    //         .Setup(x => x.GetProductByIdAsync(ProductId.Create(productId), CancellationToken.None))
    //         .ReturnsAsync((Product)null);

    //     // Act
    //     var result = await _sut.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.False(result.Succeeded);
    //     Assert.Equal(CustomErrors.Product.ProductNotFound, result.Errors.First());
    // }

    // [Fact]
    // public async Task ShouldReturnErrorResponseWhenImageIdIsNotFound()
    // {
    //     // Arrange
    //     var productId = ProductId.Create("valid-product-id");
    //     var imageId = ProductImageId.Create("non-existent-image-id");
    //     var command = new DeleteProductImageCommand(productId.Value, imageId.Value);

    //     var product = new Product(productId, "Test Product");
    //     product.AddProductImage(new ProductImage(ProductImageId.Create("image1"), "publicId1"));
    //     product.AddProductImage(new ProductImage(ProductImageId.Create("image2"), "publicId2"));

    //     _mockProductRepository
    //         .Setup(x => x.GetProductByIdAsync(productId, CancellationToken.None))
    //         .ReturnsAsync(product);

    //     _mockMediaUploader
    //         .Setup(x => x.DeletePhotoAsync(It.IsAny<string>(), CancellationToken.None))
    //         .ReturnsAsync(Result<Success>.SuccessResponese("Success"));

    //     _mockProductRepository.Setup(x => x.Update(product));

    //     _mockProductRepository.Setup(x => x.Commit(CancellationToken.None)).ReturnsAsync(1);

    //     // Act
    //     var result = await _sut.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.False(result.Succeeded);
    //     Assert.Equal(CustomErrors.Product.ProductImageNotFound, result.Errors.First());
    // }

    // [Fact]
    // public async Task ShouldDeleteTheProductImageFromTheMediaUploader()
    // {
    //     // Arrange
    //     var productId = ProductId.Create("valid-product-id");
    //     var imageId = ProductImageId.Create("valid-image-id");
    //     var command = new DeleteProductImageCommand(productId.Value, imageId.Value);

    //     var product = new Product(productId, "Test Product");
    //     product.AddProductImage(new ProductImage(ProductImageId.Create("image1"), "publicId1"));
    //     product.AddProductImage(new ProductImage(imageId, "publicId2"));

    //     _mockProductRepository
    //         .Setup(x => x.GetProductByIdAsync(productId, CancellationToken.None))
    //         .ReturnsAsync(product);

    //     _mockMediaUploader
    //         .Setup(x => x.DeletePhotoAsync("publicId2", CancellationToken.None))
    //         .ReturnsAsync(Result<Success>.SuccessResponese("Success"));

    //     // Act
    //     await _sut.Handle(command, CancellationToken.None);

    //     // Assert
    //     _mockMediaUploader.Verify(
    //         x => x.DeletePhotoAsync("publicId2", CancellationToken.None),
    //         Times.Once
    //     );
    // }

    // [Fact]
    // public async Task ShouldRemoveProductImageFromProduct()
    // {
    //     // Arrange
    //     var productId = ProductId.Create("valid-product-id");
    //     var imageId = ProductImageId.Create("valid-image-id");
    //     var command = new DeleteProductImageCommand(productId.Value, imageId.Value);

    //     var product = new Product(productId, "Test Product");
    //     product.AddProductImage(new ProductImage(ProductImageId.Create("image1"), "publicId1"));
    //     product.AddProductImage(new ProductImage(imageId, "publicId2"));

    //     _mockProductRepository
    //         .Setup(x => x.GetProductByIdAsync(productId, CancellationToken.None))
    //         .ReturnsAsync(product);

    //     _mockMediaUploader
    //         .Setup(x => x.DeletePhotoAsync("publicId2", CancellationToken.None))
    //         .ReturnsAsync(Result<Success>.SuccessResponese("Success"));

    //     _mockProductRepository.Setup(x => x.Update(product));

    //     _mockProductRepository.Setup(x => x.Commit(CancellationToken.None)).ReturnsAsync(1);

    //     // Act
    //     var result = await _sut.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.True(result.Succeeded);
    //     Assert.Equal("Delete product image successfully.", result.Data.Message);
    //     Assert.DoesNotContain(product.ProductImages, pi => pi.Id == imageId);
    // }

    // [Fact]
    // public async Task ShouldCommitChangesToProductRepository()
    // {
    //     // Arrange
    //     var productId = ProductId.Create("valid-product-id");
    //     var imageId = ProductImageId.Create("valid-image-id");
    //     var command = new DeleteProductImageCommand(productId.Value, imageId.Value);

    //     var product = new Product(productId, "Test Product");
    //     product.AddProductImage(new ProductImage(ProductImageId.Create("image1"), "publicId1"));
    //     product.AddProductImage(new ProductImage(imageId, "publicId2"));

    //     _mockProductRepository
    //         .Setup(x => x.GetProductByIdAsync(productId, CancellationToken.None))
    //         .ReturnsAsync(product);

    //     _mockMediaUploader
    //         .Setup(x => x.DeletePhotoAsync("publicId2", CancellationToken.None))
    //         .ReturnsAsync(Result<Success>.SuccessResponese("Success"));

    //     _mockProductRepository.Setup(x => x.Update(product));

    //     _mockProductRepository.Setup(x => x.Commit(CancellationToken.None)).ReturnsAsync(1);

    //     // Act
    //     await _sut.Handle(command, CancellationToken.None);

    //     // Assert
    //     _mockProductRepository.Verify(x => x.Commit(CancellationToken.None), Times.Once);
    // }

    // [Fact]
    // public async Task ShouldReturnSuccessResponseWhenOperationIsSuccessful()
    // {
    //     // Arrange
    //     var productId = ProductId.Create("valid-product-id");
    //     var imageId = ProductImageId.Create("valid-image-id");
    //     var command = new DeleteProductImageCommand(productId.Value, imageId.Value);

    //     var product = new Product(productId, "Test Product");
    //     product.AddProductImage(new ProductImage(ProductImageId.Create("image1"), "publicId1"));
    //     product.AddProductImage(new ProductImage(imageId, "publicId2"));

    //     _mockProductRepository
    //         .Setup(x => x.GetProductByIdAsync(productId, CancellationToken.None))
    //         .ReturnsAsync(product);

    //     _mockMediaUploader
    //         .Setup(x => x.DeletePhotoAsync("publicId2", CancellationToken.None))
    //         .ReturnsAsync(Result<Success>.SuccessResponese("Success"));

    //     _mockProductRepository.Setup(x => x.Update(product));

    //     _mockProductRepository.Setup(x => x.Commit(CancellationToken.None)).ReturnsAsync(1);

    //     // Act
    //     var result = await _sut.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.True(result.Succeeded);
    //     Assert.Equal("Delete product image successfully.", result.Data.Message);
    // }

    // [Fact]
    // public async Task ShouldReturnErrorResponseWhenMediaUploaderFailsToDeleteImage()
    // {
    //     // Arrange
    //     var productId = ProductId.Create("valid-product-id");
    //     var imageId = ProductImageId.Create("valid-image-id");
    //     var command = new DeleteProductImageCommand(productId.Value, imageId.Value);

    //     var product = new Product(productId, "Test Product");
    //     product.AddProductImage(new ProductImage(ProductImageId.Create("image1"), "publicId1"));
    //     product.AddProductImage(new ProductImage(imageId, "publicId2"));

    //     _mockProductRepository
    //         .Setup(x => x.GetProductByIdAsync(productId, CancellationToken.None))
    //         .ReturnsAsync(product);

    //     _mockMediaUploader
    //         .Setup(x => x.DeletePhotoAsync("publicId2", CancellationToken.None))
    //         .ReturnsAsync(Result<Success>.ErrorResponse("Failed to delete photo"));

    //     // Act
    //     var result = await _sut.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.False(result.Succeeded);
    //     Assert.Equal("Failed to delete photo", result.Errors.FirstOrDefault()?.Description);
    // }

    // [Fact]
    // public async Task ShouldReturnErrorResponseWhenProductRepositoryFailsToCommitChanges()
    // {
    //     // Arrange
    //     var productId = ProductId.Create("valid-product-id");
    //     var imageId = ProductImageId.Create("valid-image-id");
    //     var command = new DeleteProductImageCommand(productId.Value, imageId.Value);

    //     var product = new Product(productId, "Test Product");
    //     product.AddProductImage(new ProductImage(ProductImageId.Create("image1"), "publicId1"));
    //     product.AddProductImage(new ProductImage(imageId, "publicId2"));

    //     _mockProductRepository
    //         .Setup(x => x.GetProductByIdAsync(productId, CancellationToken.None))
    //         .ReturnsAsync(product);

    //     _mockMediaUploader
    //         .Setup(x => x.DeletePhotoAsync("publicId2", CancellationToken.None))
    //         .ReturnsAsync(Result<Success>.SuccessResponese("Success"));

    //     _mockProductRepository.Setup(x => x.Update(product));

    //     _mockProductRepository.Setup(x => x.Commit(CancellationToken.None)).ReturnsAsync(0); // Simulate failure to commit changes

    //     // Act
    //     var result = await _sut.Handle(command, CancellationToken.None);

    //     // Assert
    //     Assert.False(result.Succeeded);
    //     Assert.Equal(CustomErrors.Product.ProductImageNotAdded, result.Errors.First());
    // }

    // [Fact]
    // public async Task ShouldHandleConcurrentRequestsAndEnsureDataIntegrity()
    // {
    //     // Arrange
    //     var productId = ProductId.Create("valid-product-id");
    //     var imageId = ProductImageId.Create("valid-image-id");
    //     var command = new DeleteProductImageCommand(productId.Value, imageId.Value);

    //     var productRepositoryMock = new Mock<IProductRepository>();
    //     var mediaUploaderMock = new Mock<IMediaUploader>();
    //     var sut = new DeleteProductImageCommandHandler(
    //         productRepositoryMock.Object,
    //         mediaUploaderMock.Object
    //     );

    //     var product = new Product(productId, "Test Product");
    //     product.AddProductImage(new ProductImage(ProductImageId.Create("image1"), "publicId1"));
    //     product.AddProductImage(new ProductImage(imageId, "publicId2"));

    //     productRepositoryMock
    //         .Setup(x => x.GetProductByIdAsync(productId, CancellationToken.None))
    //         .ReturnsAsync(product);

    //     mediaUploaderMock
    //         .Setup(x => x.DeletePhotoAsync("publicId2", CancellationToken.None))
    //         .ReturnsAsync(Result<Success>.SuccessResponese("Success"));

    //     productRepositoryMock.Setup(x => x.Update(product));

    //     productRepositoryMock
    //         .Setup(x => x.Commit(CancellationToken.None))
    //         .ReturnsAsync(1)
    //         .Callback(() => Thread.Sleep(100)); // Simulate concurrent request

    //     // Act
    //     var task1 = sut.Handle(command, CancellationToken.None);
    //     var task2 = sut.Handle(command, CancellationToken.None);

    //     await Task.WhenAll(task1, task2);

    //     // Assert
    //     Assert.True(task1.Result.Succeeded);
    //     Assert.True(task2.Result.Succeeded);
    //     Assert.Equal("Delete product image successfully.", task1.Result.Data.Message);
    //     Assert.Equal("Delete product image successfully.", task2.Result.Data.Message);
    // }
}

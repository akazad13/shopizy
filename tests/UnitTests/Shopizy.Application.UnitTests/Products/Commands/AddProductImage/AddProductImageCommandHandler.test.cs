using FluentAssertions;
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
    public async Task ShouldThrowExceptionWhenFileIsNullAsync()
    {
        // Arrange

        var command = new AddProductImageCommand(
            Constants.User.Id.Value,
            Constants.Product.Id.Value,
            null
        );

        // Act
        var result = (await _sut.Handle(command, CancellationToken.None)).Match(x => null, x => x);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Success>();
        result.Errors.Should().Contain(CustomErrors.Product.ProductImageNotUploaded);
    }

    [Fact]
    public async Task ShouldReturnErrorResponseWhenProductIsNotFoundAsync()
    {
        // Arrange
        var command = AddProductImageCommandUtils.CreateCommand(Constants.Product.Id.Value);

        _mockProductRepository
            .Setup(x => x.GetProductByIdAsync(It.IsAny<ProductId>()))
            .ReturnsAsync(() => null);

        // Act
        var result = (await _sut.Handle(command, CancellationToken.None)).Match(x => null, x => x);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Success>();
        result.Errors.Should().Contain(CustomErrors.Product.ProductNotFound);
    }

    [Fact]
    public async Task ShouldReturnProductImageWhenProductIsFoundAndImageIsUploadedAsync()
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
                () =>
                    Response<PhotoUploadResult>.SuccessResponese(
                        new PhotoUploadResult(
                            Constants.ProductImage.ImageUrl,
                            Constants.ProductImage.PublicId
                        )
                    )
            );

        _mockProductRepository.Setup(p => p.Update(product));
        _mockProductRepository.Setup(p => p.Commit(default)).ReturnsAsync(1);
        // Act
        var result = (await _sut.Handle(command, CancellationToken.None)).Match(
            x => x,
            x => productImage
        );

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType(typeof(ProductImage));
        result.ImageUrl.Should().Be(productImage.ImageUrl);
        result.PublicId.Should().Be(productImage.PublicId);

        _mockProductRepository.Verify(m => m.Commit(default), Times.Once);
    }

    // [Fact]
    // public async Task ShouldHandleConcurrentRequestsWithoutDataCorruption()
    // {
    //     // Arrange
    //     var mockProductRepository = new Mock<IProductRepository>();
    //     var mockMediaUploader = new Mock<IMediaUploader>();

    //     var handler = new AddProductImageCommandHandler(
    //         mockProductRepository.Object,
    //         mockMediaUploader.Object
    //     );

    //     var command = new AddProductImageCommand
    //     {
    //         File = new Mock<IFormFile>().Object,
    //         ProductId = "123",
    //     };

    //     var product = Product.Create(ProductId.Create("123"), "Test Product", 100m);
    //     mockProductRepository
    //         .Setup(x => x.GetProductByIdAsync(It.IsAny<ProductId>()))
    //         .ReturnsAsync(product);

    //     mockMediaUploader
    //         .Setup(x => x.UploadPhotoAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(
    //             Result<PhotoUploadResult>.Success(new PhotoUploadResult("url", "publicId"))
    //         );

    //     var tasks = new List<Task<ErrorOr<ProductImage>>>();

    //     for (int i = 0; i < 10; i++)
    //     {
    //         tasks.Add(handler.Handle(command, CancellationToken.None));
    //     }

    //     // Act
    //     var results = await Task.WhenAll(tasks);

    //     // Assert
    //     foreach (var result in results)
    //     {
    //         result.IsSuccess.Should().BeTrue();
    //         result.Value.Url.Should().Be("url");
    //         result.Value.PublicId.Should().Be("publicId");
    //         result.Value.SortOrder.Should().Be(product.ProductImages.Count);
    //     }

    //     mockProductRepository.Verify(x => x.Update(It.IsAny<Product>()), Times.Exactly(10));
    //     mockProductRepository.Verify(
    //         x => x.Commit(It.IsAny<CancellationToken>()),
    //         Times.Exactly(10)
    //     );
    // }

    // [Fact]
    // public async Task ShouldValidateMaximumAllowedNumberOfProductImages()
    // {
    //     // Arrange
    //     var mockProductRepository = new Mock<IProductRepository>();
    //     var mockMediaUploader = new Mock<IMediaUploader>();

    //     var product = new Product("123", "Test Product", "Test Description", 100m);
    //     for (int i = 0; i < 10; i++)
    //     {
    //         product.AddProductImage(ProductImage.Create("url", i, "publicId"));
    //     }

    //     mockProductRepository
    //         .Setup(x => x.GetProductByIdAsync(It.IsAny<ProductId>()))
    //         .ReturnsAsync(product);

    //     var handler = new AddProductImageCommandHandler(
    //         mockProductRepository.Object,
    //         mockMediaUploader.Object
    //     );

    //     var command = new AddProductImageCommand
    //     {
    //         File = new Mock<IFormFile>().Object,
    //         ProductId = "123",
    //     };

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     result.IsFailure.Should().BeTrue();
    //     result.Errors.Should().Contain(CustomErrors.Product.MaximumProductImagesReached);
    // }

    // [Fact]
    // public async Task ShouldCorrectlyUpdateProductImageCount()
    // {
    //     // Arrange
    //     var mockProductRepository = new Mock<IProductRepository>();
    //     var mockMediaUploader = new Mock<IMediaUploader>();

    //     var existingProduct = new Product(
    //         ProductId.Create("123"),
    //         "Test Product",
    //         "Test Description",
    //         new List<ProductImage>()
    //     );

    //     mockProductRepository
    //         .Setup(x => x.GetProductByIdAsync(It.IsAny<ProductId>()))
    //         .ReturnsAsync(existingProduct);

    //     var handler = new AddProductImageCommandHandler(
    //         mockProductRepository.Object,
    //         mockMediaUploader.Object
    //     );

    //     var command = new AddProductImageCommand
    //     {
    //         File = new Mock<IFormFile>().Object,
    //         ProductId = "123",
    //     };

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     result.IsSuccess.Should().BeTrue();
    //     existingProduct.ProductImages.Count.Should().Be(1);
    // }

    // [Fact]
    // public async Task ShouldHandleMediaUploadFailuresGracefully()
    // {
    //     // Arrange
    //     var mockProductRepository = new Mock<IProductRepository>();
    //     var mockMediaUploader = new Mock<IMediaUploader>();

    //     mockMediaUploader
    //         .Setup(x => x.UploadPhotoAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(
    //             Result<PhotoUploadResult>.Failure(new PhotoUploadError("Failed to upload photo"))
    //         );

    //     var handler = new AddProductImageCommandHandler(
    //         mockProductRepository.Object,
    //         mockMediaUploader.Object
    //     );

    //     var command = new AddProductImageCommand
    //     {
    //         File = new Mock<IFormFile>().Object,
    //         ProductId = "123",
    //     };

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     result.IsFailure.Should().BeTrue();
    //     result.Errors.Should().Contain(CustomErrors.Product.UnableToAddProductImage);
    // }

    // [Fact]
    // public async Task ShouldRollbackChangesWhenCommitFails()
    // {
    //     // Arrange
    //     var mockProductRepository = new Mock<IProductRepository>();
    //     var mockMediaUploader = new Mock<IMediaUploader>();

    //     mockProductRepository
    //         .Setup(x => x.GetProductByIdAsync(It.IsAny<ProductId>()))
    //         .ReturnsAsync(new Product());

    //     mockMediaUploader
    //         .Setup(x => x.UploadPhotoAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(
    //             Response<PhotoUploadResult>.SuccessResponese(
    //                 new PhotoUploadResult("url", "publicId")
    //             )
    //         );

    //     mockProductRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(0);

    //     var handler = new AddProductImageCommandHandler(
    //         mockProductRepository.Object,
    //         mockMediaUploader.Object
    //     );

    //     var command = new AddProductImageCommand
    //     {
    //         File = new Mock<IFormFile>().Object,
    //         ProductId = "123",
    //     };

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     result.IsFailure.Should().BeTrue();
    //     result.Errors.Should().Contain(CustomErrors.Product.ProductImageNotAdded);
    //     mockProductRepository.Verify(x => x.Update(It.IsAny<Product>()), Times.Once);
    //     mockProductRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    // }

    // [Fact]
    // public async Task ShouldHandleLargeFilesEfficiently()
    // {
    //     // Arrange
    //     var mockProductRepository = new Mock<IProductRepository>();
    //     var mockMediaUploader = new Mock<IMediaUploader>();

    //     var handler = new AddProductImageCommandHandler(
    //         mockProductRepository.Object,
    //         mockMediaUploader.Object
    //     );

    //     var largeFile = new Mock<IFormFile>();
    //     largeFile.Setup(x => x.Length).Returns(10 * 1024 * 1024); // 10MB

    //     var command = new AddProductImageCommand { File = largeFile.Object, ProductId = "123" };

    //     mockProductRepository
    //         .Setup(x => x.GetProductByIdAsync(It.IsAny<ProductId>()))
    //         .ReturnsAsync(new Product("Test Product", "Test Description", 100));

    //     mockMediaUploader
    //         .Setup(x => x.UploadPhotoAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(
    //             Result<PhotoUploadResult>.Success(
    //                 new PhotoUploadResult("test-url", "test-public-id")
    //             )
    //         );

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     result.IsSuccess.Should().BeTrue();
    // }

    // [Fact]
    // public async Task ShouldSupportMultipleMediaUploadProviders()
    // {
    //     // Arrange
    //     var mockProductRepository = new Mock<IProductRepository>();
    //     var mockMediaUploader1 = new Mock<IMediaUploader>();
    //     var mockMediaUploader2 = new Mock<IMediaUploader>();

    //     var handler = new AddProductImageCommandHandler(
    //         mockProductRepository.Object,
    //         mockMediaUploader1.Object
    //     );

    //     var command = new AddProductImageCommand
    //     {
    //         File = new Mock<IFormFile>().Object,
    //         ProductId = "123",
    //     };

    //     mockProductRepository
    //         .Setup(x => x.GetProductByIdAsync(It.IsAny<ProductId>()))
    //         .ReturnsAsync(new Product("Test Product", "123"));

    //     mockMediaUploader1
    //         .Setup(x => x.UploadPhotoAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(
    //             Result<PhotoUploadResult>.Success(new PhotoUploadResult("url1", "publicId1"))
    //         );

    //     mockMediaUploader2
    //         .Setup(x => x.UploadPhotoAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(
    //             Result<PhotoUploadResult>.Success(new PhotoUploadResult("url2", "publicId2"))
    //         );

    //     // Act
    //     var result1 = await handler.Handle(command, CancellationToken.None);
    //     handler = new AddProductImageCommandHandler(
    //         mockProductRepository.Object,
    //         mockMediaUploader2.Object
    //     );
    //     var result2 = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     result1.IsSuccess.Should().BeTrue();
    //     result2.IsSuccess.Should().BeTrue();

    //     mockMediaUploader1.Verify(
    //         x => x.UploadPhotoAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()),
    //         Times.Once
    //     );
    //     mockMediaUploader2.Verify(
    //         x => x.UploadPhotoAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()),
    //         Times.Once
    //     );
    // }

    // [Fact]
    // public async Task ShouldEnsureDataConsistencyAcrossMultipleRepositories()
    // {
    //     // Arrange
    //     var mockProductRepository = new Mock<IProductRepository>();
    //     var mockMediaUploader = new Mock<IMediaUploader>();

    //     var handler = new AddProductImageCommandHandler(
    //         mockProductRepository.Object,
    //         mockMediaUploader.Object
    //     );

    //     var command = new AddProductImageCommand
    //     {
    //         File = new Mock<IFormFile>().Object,
    //         ProductId = "123",
    //     };

    //     var product = new Product(
    //         ProductId.Create("123"),
    //         "Test Product",
    //         "Test Description",
    //         new List<ProductImage>(),
    //         new List<ProductVariant>()
    //     );

    //     mockProductRepository
    //         .Setup(x => x.GetProductByIdAsync(It.IsAny<ProductId>()))
    //         .ReturnsAsync(product);

    //     mockProductRepository
    //         .Setup(x => x.Update(It.IsAny<Product>()))
    //         .Callback<Product>(p => product = p);

    //     mockProductRepository.Setup(x => x.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

    //     // Act
    //     var result = await handler.Handle(command, CancellationToken.None);

    //     // Assert
    //     result.IsSuccess.Should().BeTrue();
    //     mockProductRepository.Verify(x => x.Update(It.IsAny<Product>()), Times.Once);
    //     mockProductRepository.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
    // }
}

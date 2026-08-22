using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Moq;
using Shopizy.Infrastructure.ExternalServices.MediaUploader.CloudinaryService;
using Shouldly;
using Xunit;

namespace Shopizy.Infrastructure.UnitTests.ExternalServices.MediaUploader;

public class CloudinaryMediaUploaderTests
{
    private readonly Mock<ICloudinary> _mockCloudinary;
    private readonly CloudinaryMediaUploader _uploader;

    public CloudinaryMediaUploaderTests()
    {
        _mockCloudinary = new Mock<ICloudinary>();
        _uploader = new CloudinaryMediaUploader(_mockCloudinary.Object);
    }

    [Fact]
    public async Task UploadPhotoAsync_WithValidFile_ShouldReturnSuccess()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        var content = "test content";
        var fileName = "test.jpg";
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(content);
        writer.Flush();
        ms.Position = 0;

        fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
        fileMock.Setup(_ => _.FileName).Returns(fileName);
        fileMock.Setup(_ => _.Length).Returns(ms.Length);

        var uploadResult = new ImageUploadResult
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Url = new Uri("http://example.com/test.jpg"),
            PublicId = "test_id",
        };

        _mockCloudinary
            .Setup(c => c.UploadAsync(It.IsAny<ImageUploadParams>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(uploadResult);

        // Act
        var result = await _uploader.UploadPhotoAsync(fileMock.Object, CancellationToken.None);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.Url.ShouldBe("http://example.com/test.jpg");
        result.Value.PublicId.ShouldBe("test_id");
    }

    [Fact]
    public async Task UploadPhotoAsync_WithZeroLengthFile_ShouldReturnError()
    {
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(_ => _.Length).Returns(0);

        var result = await _uploader.UploadPhotoAsync(fileMock.Object, CancellationToken.None);

        result.IsError.ShouldBeTrue();
        result.FirstError.Description.ShouldBe("File not found!");
    }

    [Fact]
    public async Task UploadPhotoAsync_WithFileTooLarge_ShouldReturnError()
    {
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(_ => _.Length).Returns(11 * 1024 * 1024); // 11MB

        var result = await _uploader.UploadPhotoAsync(fileMock.Object, CancellationToken.None);

        result.IsError.ShouldBeTrue();
        result.FirstError.Description.ShouldContain("exceeds the maximum allowed size");
    }

    [Fact]
    public async Task UploadPhotoAsync_WhenCloudinaryReturnsError_ShouldReturnError()
    {
        var fileMock = new Mock<IFormFile>();
        var ms = new MemoryStream([1, 2, 3]);
        fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
        fileMock.Setup(_ => _.Length).Returns(3);

        var uploadResult = new ImageUploadResult
        {
            Error = new Error { Message = "Upload service rejected image" },
        };

        _mockCloudinary
            .Setup(c => c.UploadAsync(It.IsAny<ImageUploadParams>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(uploadResult);

        var result = await _uploader.UploadPhotoAsync(fileMock.Object, CancellationToken.None);

        result.IsError.ShouldBeTrue();
        result.FirstError.Description.ShouldBe("Upload service rejected image");
    }

    [Fact]
    public async Task UploadPhotoAsync_WhenExceptionThrown_ShouldReturnError()
    {
        var fileMock = new Mock<IFormFile>();
        var ms = new MemoryStream([1, 2, 3]);
        fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
        fileMock.Setup(_ => _.Length).Returns(3);

        _mockCloudinary
            .Setup(c => c.UploadAsync(It.IsAny<ImageUploadParams>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Fatal error"));

        var result = await _uploader.UploadPhotoAsync(fileMock.Object, CancellationToken.None);

        result.IsError.ShouldBeTrue();
        result.FirstError.Description.ShouldBe("Fatal error");
    }

    [Fact]
    public async Task DeletePhotoAsync_WithValidId_ShouldReturnSuccess()
    {
        var publicId = "test_id";
        var deletionResult = new DeletionResult { Result = "ok" };

        _mockCloudinary
            .Setup(c => c.DestroyAsync(It.IsAny<DeletionParams>()))
            .ReturnsAsync(deletionResult);

        var result = await _uploader.DeletePhotoAsync(publicId);

        result.IsError.ShouldBeFalse();
    }

    [Fact]
    public async Task DeletePhotoAsync_WhenDeletionFails_ShouldReturnError()
    {
        var publicId = "test_id";
        var deletionResult = new DeletionResult
        {
            Result = "not found",
            Error = new Error { Message = "Image not found" },
        };

        _mockCloudinary
            .Setup(c => c.DestroyAsync(It.IsAny<DeletionParams>()))
            .ReturnsAsync(deletionResult);

        var result = await _uploader.DeletePhotoAsync(publicId);

        result.IsError.ShouldBeTrue();
        result.FirstError.Description.ShouldBe("Image not found");
    }

    [Fact]
    public async Task DeletePhotoAsync_WhenExceptionThrown_ShouldReturnError()
    {
        _mockCloudinary
            .Setup(c => c.DestroyAsync(It.IsAny<DeletionParams>()))
            .ThrowsAsync(new InvalidOperationException("Delete connection error"));

        var result = await _uploader.DeletePhotoAsync("test_id");

        result.IsError.ShouldBeTrue();
        result.FirstError.Description.ShouldBe("Delete connection error");
    }
}

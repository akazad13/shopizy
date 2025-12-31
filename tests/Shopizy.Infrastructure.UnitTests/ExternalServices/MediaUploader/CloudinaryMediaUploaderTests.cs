using Xunit;
using Moq;
using Shouldly;
using Microsoft.AspNetCore.Http;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Shopizy.Infrastructure.ExternalServices.MediaUploader.CloudinaryService;
using Shopizy.Application.Products.Common;

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
            PublicId = "test_id"
        };

        _mockCloudinary.Setup(c => c.UploadAsync(It.IsAny<ImageUploadParams>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(uploadResult);

        // Act
        var result = await _uploader.UploadPhotoAsync(fileMock.Object);

        // Assert
        result.IsError.ShouldBeFalse();
        result.Value.Url.ShouldBe("http://example.com/test.jpg");
        result.Value.PublicId.ShouldBe("test_id");
    }

    [Fact]
    public async Task DeletePhotoAsync_WithValidId_ShouldReturnSuccess()
    {
        // Arrange
        var publicId = "test_id";
        var deletionResult = new DeletionResult
        {
            Result = "ok"
        };

        _mockCloudinary.Setup(c => c.DestroyAsync(It.IsAny<DeletionParams>()))
            .ReturnsAsync(deletionResult);

        // Act
        var result = await _uploader.DeletePhotoAsync(publicId);

        // Assert
        result.IsError.ShouldBeFalse();
    }
}

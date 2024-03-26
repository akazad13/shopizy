using System.Text;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ErrorOr;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Shopizy.Application.Products.Common;
using Shopizy.Infrastructure.ExternalServices.MediaUploader.CloudinaryService;
using Xunit;

namespace Shopizy.Infrastructure.UnitTests.ExternalServices.MediaUploader.CloudinaryService;

public class CloudinaryMediaUploaderTests
{
    private readonly Mock<ICloudinary> _cloudinary;
    private readonly CloudinaryMediaUploader _cloudinaryMediaUploader;

    public CloudinaryMediaUploaderTests()
    {
        _cloudinary = new Mock<ICloudinary>();
        _cloudinaryMediaUploader = new CloudinaryMediaUploader(_cloudinary.Object);
    }

    [Fact]
    public async Task UploadPhoto_WithValidFile_ShouldReturnSecureUrl()
    {
        // Arrange
        var filebytes = Encoding.UTF8.GetBytes("dummy image");
        IFormFile file = new FormFile(
            new MemoryStream(filebytes),
            0,
            filebytes.Length,
            "Data",
            "image.png"
        );

        var expectedUrl = "https://res.cloudinary.com/test/image/upload/test";
        var expectedPublicId = "test";

        _cloudinary
            .Setup(c => c.UploadAsync(It.IsAny<ImageUploadParams>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new ImageUploadResult { Url = new Uri(expectedUrl), PublicId = expectedPublicId }
            );

        // Act
        var result = await _cloudinaryMediaUploader.UploadPhotoAsync(file);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType(typeof(PhotoUploadResult));
        result.Value.Should().NotBeNull();
        result.Value.Url.ToString().Should().Be(expectedUrl);
        result.Value.PublicId.Should().Be(expectedPublicId);
        _cloudinary.Verify(
            c => c.UploadAsync(It.IsAny<ImageUploadParams>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task DeletePhoto_WithValidPublicId_ShouldDeleteAndReturnBoolean()
    {
        // Arrange
        var publicId = "test";
        _cloudinary
            .Setup(c => c.DestroyAsync(It.IsAny<DeletionParams>()))
            .ReturnsAsync(new DeletionResult { Result = "ok" });

        // Act
        var result = await _cloudinaryMediaUploader.DeletePhotoAsync(publicId);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeOfType(typeof(Success));
        _cloudinary.Verify(c => c.DestroyAsync(It.IsAny<DeletionParams>()), Times.Once);
    }
}

using System.Text;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ErrorOr;
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
    public async Task ShouldReturnReturnSecureUrlWithValidFile()
    {
        // Arrange
        byte[] filebytes = Encoding.UTF8.GetBytes("dummy image");
        var file = new FormFile(
            new MemoryStream(filebytes),
            0,
            filebytes.Length,
            "Data",
            "image.png"
        );

        string expectedUrl = "https://res.cloudinary.com/test/image/upload/test";
        string expectedPublicId = "test";

        _cloudinary
            .Setup(c => c.UploadAsync(It.IsAny<ImageUploadParams>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new ImageUploadResult { Url = new Uri(expectedUrl), PublicId = expectedPublicId }
            );

        // Act
        var result = await _cloudinaryMediaUploader.UploadPhotoAsync(file, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsError);
        Assert.IsType<PhotoUploadResult>(result.Value);
        Assert.NotNull(result.Value);
        Assert.Equal(expectedUrl, result.Value.Url.ToString());
        Assert.Equal(expectedPublicId, result.Value.PublicId);
        _cloudinary.Verify(
            c => c.UploadAsync(It.IsAny<ImageUploadParams>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task ShouldDeleteAndReturnSuccessWithValidPublicId()
    {
        // Arrange
        string publicId = "test";
        _cloudinary
            .Setup(c => c.DestroyAsync(It.IsAny<DeletionParams>()))
            .ReturnsAsync(new DeletionResult { Result = "ok" });

        // Act
        var result = await _cloudinaryMediaUploader.DeletePhotoAsync(publicId);

        // Assert
        Assert.False(result.IsError);
        Assert.IsType<Success>(result.Value);
        _cloudinary.Verify(c => c.DestroyAsync(It.IsAny<DeletionParams>()), Times.Once);
    }
}

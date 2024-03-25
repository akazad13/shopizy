using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ErrorOr;
using Microsoft.AspNetCore.Http;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Products.Common;

namespace Shopizy.Infrastructure.MediaUploader.CloudinaryService;

public class CloudinaryMediaUploader(
    ICloudinary cloudinary
    ) : ICloudinaryMediaUploader
{
    private readonly ICloudinary _cloudinary = cloudinary;

    public async Task<ErrorOr<PhotoUploadResult>> UploadPhotoAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        try
        {
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.Name, stream),
                    Transformation = new Transformation().Width(500).Height(500).Crop("fill")
                    // .Gravity("face")
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams, cancellationToken);

                if (uploadResult.Error != null)
                {
                    return ErrorOr.Error.Failure(description: uploadResult.Error.Message);
                }

                return new PhotoUploadResult (
                    uploadResult.Url.ToString(), uploadResult.PublicId);
            }
            return ErrorOr.Error.Failure();
        }
        catch (Exception ex)
        {
            return ErrorOr.Error.Failure(description: ex.Message);
        }
    }
}
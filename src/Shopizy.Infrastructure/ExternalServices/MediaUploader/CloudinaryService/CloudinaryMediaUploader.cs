using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ErrorOr;
using Microsoft.AspNetCore.Http;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Products.Common;

namespace Shopizy.Infrastructure.ExternalServices.MediaUploader.CloudinaryService;

public class CloudinaryMediaUploader(ICloudinary cloudinary) : IMediaUploader
{
    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

    private readonly ICloudinary _cloudinary = cloudinary;

    public async Task<ErrorOr<PhotoUploadResult>> UploadPhotoAsync(
        IFormFile file,
        CancellationToken cancellationToken = default
    )
    {
        if (file.Length == 0)
        {
            return ErrorOr.Error.Failure(description: "File not found!");
        }

        if (file.Length > MaxFileSizeBytes)
        {
            return ErrorOr.Error.Failure(
                description: $"File size exceeds the maximum allowed size of {MaxFileSizeBytes / (1024 * 1024)} MB."
            );
        }

        var maxAttempts = 3;
        for (var attempt = 0; attempt < maxAttempts; attempt++)
        {
            try
            {
                using Stream stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.Name, stream),
                    Transformation = new Transformation().Width(500).Height(500).Crop("fill"),
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams, cancellationToken);

                return uploadResult.Error switch
                {
                    null => new PhotoUploadResult(
                        uploadResult.Url.ToString(),
                        uploadResult.PublicId
                    ),
                    _ => ErrorOr.Error.Failure(description: uploadResult.Error.Message),
                };
            }
            catch (Exception ex) when (
                ex is HttpRequestException or TaskCanceledException or TimeoutException
                && attempt < maxAttempts - 1)
            {
                await Task.Delay(
                    TimeSpan.FromMilliseconds(500 * Math.Pow(2, attempt)),
                    cancellationToken
                );
            }
            catch (Exception ex)
            {
                return ErrorOr.Error.Failure(description: ex.Message);
            }
        }

        return ErrorOr.Error.Failure(
            description: "Photo upload failed after maximum retry attempts."
        );
    }

    public async Task<ErrorOr<Success>> DeletePhotoAsync(string publicId)
    {
        try
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result.Result switch
            {
                "ok" => Result.Success,
                _ => ErrorOr.Error.Failure(description: result.Error.Message),
            };
        }
        catch (Exception ex)
        {
            return ErrorOr.Error.Failure(description: ex.Message);
        }
    }
}

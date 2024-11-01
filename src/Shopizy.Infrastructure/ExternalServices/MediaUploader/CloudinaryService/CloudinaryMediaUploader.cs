using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Application.Products.Common;

namespace Shopizy.Infrastructure.ExternalServices.MediaUploader.CloudinaryService;

public class CloudinaryMediaUploader(ICloudinary cloudinary) : IMediaUploader
{
    private readonly ICloudinary _cloudinary = cloudinary;

    public async Task<IResult<PhotoUploadResult>> UploadPhotoAsync(
        IFormFile file,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            if (file.Length > 0)
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
                    null => Response<PhotoUploadResult>.SuccessResponese(
                        new PhotoUploadResult(uploadResult.Url.ToString(), uploadResult.PublicId)
                    ),
                    _ => Response<PhotoUploadResult>.ErrorResponse([uploadResult.Error.Message]),
                };
            }
            return Response<PhotoUploadResult>.ErrorResponse(["File not found!"]);
        }
        catch (Exception ex)
        {
            return Response<PhotoUploadResult>.ErrorResponse([ex.Message]);
        }
    }

    public async Task<Result> DeletePhotoAsync(string publicId)
    {
        try
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result.Result switch
            {
                "ok" => Result.Success(),
                _ => Result.Failure([result.Error.Message]),
            };
        }
        catch (Exception ex)
        {
            return Result.Failure([ex.Message]);
        }
    }
}

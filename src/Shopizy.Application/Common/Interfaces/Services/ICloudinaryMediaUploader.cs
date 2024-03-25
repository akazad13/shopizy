using ErrorOr;
using Microsoft.AspNetCore.Http;
using Shopizy.Application.Products.Common;

namespace Shopizy.Application.Common.Interfaces.Services;

public interface ICloudinaryMediaUploader
{
    Task<ErrorOr<PhotoUploadResult>> UploadPhotoAsync(
        IFormFile file,
        CancellationToken cancellationToken = default
    );
    Task<ErrorOr<bool>> DeletePhotoAsync(string publicId);
}

using Microsoft.AspNetCore.Http;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Application.Products.Common;

namespace Shopizy.Application.Common.Interfaces.Services;

public interface IMediaUploader
{
    Task<IResult<PhotoUploadResult>> UploadPhotoAsync(
        IFormFile file,
        CancellationToken cancellationToken = default
    );
    Task<Result> DeletePhotoAsync(string publicId);
}

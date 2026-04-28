using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Brands.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Brands.Commands.DeleteBrand;

public class DeleteBrandCommandHandler(IBrandRepository brandRepository)
    : ICommandHandler<DeleteBrandCommand, ErrorOr<Success>>
{
    private readonly IBrandRepository _brandRepository = brandRepository;

    public async Task<ErrorOr<Success>> Handle(
        DeleteBrandCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var brand = await _brandRepository.GetByIdAsync(BrandId.Create(command.BrandId));
        if (brand is null)
        {
            return (Error)CustomErrors.Brand.BrandNotFound;
        }

        _brandRepository.Remove(brand);

        return Result.Success;
    }
}

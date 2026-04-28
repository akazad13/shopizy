using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Brands.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Brands.Commands.UpdateBrand;

public class UpdateBrandCommandHandler(IBrandRepository brandRepository)
    : ICommandHandler<UpdateBrandCommand, ErrorOr<Success>>
{
    private readonly IBrandRepository _brandRepository = brandRepository;

    public async Task<ErrorOr<Success>> Handle(
        UpdateBrandCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var brand = await _brandRepository.GetByIdAsync(BrandId.Create(command.BrandId));
        if (brand is null)
        {
            return (Error)CustomErrors.Brand.BrandNotFound;
        }

        var brandByName = await _brandRepository.GetByNameAsync(command.Name);
        if (brandByName is not null && brandByName.Id != brand.Id)
        {
            return (Error)CustomErrors.Brand.DuplicateName;
        }

        brand.Update(command.Name, command.LogoUrl, command.Country);

        return Result.Success;
    }
}

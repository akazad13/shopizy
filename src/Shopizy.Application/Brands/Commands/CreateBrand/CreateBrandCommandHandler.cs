using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Brands;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Brands.Commands.CreateBrand;

public class CreateBrandCommandHandler(IBrandRepository brandRepository)
    : ICommandHandler<CreateBrandCommand, ErrorOr<Brand>>
{
    private readonly IBrandRepository _brandRepository = brandRepository;

    public async Task<ErrorOr<Brand>> Handle(
        CreateBrandCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var brandByName = await _brandRepository.GetByNameAsync(command.Name);
        if (brandByName is not null)
        {
            return (Error)CustomErrors.Brand.DuplicateName;
        }

        var brand = Brand.Create(command.Name, command.LogoUrl, command.Country);
        await _brandRepository.AddAsync(brand);

        return brand;
    }
}

using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Brands;
using Shopizy.Domain.Brands.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Brands.Queries.GetBrand;

public class GetBrandQueryHandler(IBrandRepository brandRepository)
    : IQueryHandler<GetBrandQuery, ErrorOr<Brand>>
{
    private readonly IBrandRepository _brandRepository = brandRepository;

    public async Task<ErrorOr<Brand>> Handle(GetBrandQuery query, CancellationToken cancellationToken)
    {
        var brand = await _brandRepository.GetByIdAsync(BrandId.Create(query.BrandId));
        if (brand is null)
        {
            return (Error)CustomErrors.Brand.BrandNotFound;
        }

        return brand;
    }
}
using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Brands.Queries.ListBrands;

public class ListBrandsQueryHandler(IBrandRepository brandRepository)
    : IQueryHandler<ListBrandsQuery, ErrorOr<List<BrandItem>>>
{
    private readonly IBrandRepository _brandRepository = brandRepository;

    public async Task<ErrorOr<List<BrandItem>>> Handle(
        ListBrandsQuery query,
        CancellationToken cancellationToken = default
    )
    {
        var brands = await _brandRepository.GetAsync();
        return brands
            .Select(brand => new BrandItem(brand.Id.Value, brand.Name, brand.LogoUrl, brand.Country))
            .ToList();
    }
}

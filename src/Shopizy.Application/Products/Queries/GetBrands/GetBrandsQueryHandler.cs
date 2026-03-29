using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Application.Common.Interfaces.Persistence;

namespace Shopizy.Application.Products.Queries.GetBrands;

public class GetBrandsQueryHandler(IProductRepository productRepository)
    : IQueryHandler<GetBrandsQuery, ErrorOr<IReadOnlyList<string>>>
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<ErrorOr<IReadOnlyList<string>>> Handle(GetBrandsQuery query, CancellationToken cancellationToken)
    {
        var brands = await _productRepository.GetBrandsAsync();
        return brands.ToErrorOr();
    }
}

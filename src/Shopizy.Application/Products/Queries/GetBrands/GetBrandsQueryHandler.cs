using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Products.Queries.GetBrands;

public class GetBrandsQueryHandler(IProductReader productReader)
    : IQueryHandler<GetBrandsQuery, ErrorOr<IReadOnlyList<string>>>
{
    private readonly IProductReader _productReader = productReader;

    public async Task<ErrorOr<IReadOnlyList<string>>> Handle(
        GetBrandsQuery query,
        CancellationToken cancellationToken
    )
    {
        var brands = await _productReader.GetBrandNamesAsync(cancellationToken);
        return brands.ToErrorOr();
    }
}

using Shopizy.Application.Products.Queries.GetProduct;
using Shopizy.Application.UnitTests.TestUtils.Constants;

namespace Shopizy.Application.UnitTests.Products.Queries.TestUtils;

public static class GetProductQueryUtils
{
    public static GetProductQuery CreateQuery()
    {
        return new GetProductQuery(Constants.Product.Id.Value);
    }
}

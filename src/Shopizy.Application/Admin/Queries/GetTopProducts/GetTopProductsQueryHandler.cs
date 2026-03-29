using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Application.Admin.Queries.GetSalesReport;
using Shopizy.Application.Common.Interfaces.Persistence;

namespace Shopizy.Application.Admin.Queries.GetTopProducts;

public class GetTopProductsQueryHandler(IOrderRepository orderRepository)
    : IQueryHandler<GetTopProductsQuery, ErrorOr<List<TopProductDto>>>
{
    private readonly IOrderRepository _orderRepository = orderRepository;

    public async Task<ErrorOr<List<TopProductDto>>> Handle(GetTopProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _orderRepository.GetTopProductsByRevenueAsync(request.Count);
        return products.ToList();
    }
}

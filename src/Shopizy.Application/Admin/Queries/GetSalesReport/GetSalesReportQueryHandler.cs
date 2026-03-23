using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Application.Common.Interfaces.Persistence;

namespace Shopizy.Application.Admin.Queries.GetSalesReport;

public class GetSalesReportQueryHandler(IOrderRepository orderRepository)
    : IQueryHandler<GetSalesReportQuery, ErrorOr<SalesReportDto>>
{
    private readonly IOrderRepository _orderRepository = orderRepository;

    public async Task<ErrorOr<SalesReportDto>> Handle(GetSalesReportQuery request, CancellationToken cancellationToken)
    {
        var revenue = await _orderRepository.GetRevenueByPeriodAsync(request.StartDate, request.EndDate);
        var orders = await _orderRepository.GetOrdersAsync(null, request.StartDate, request.EndDate, null, 1, int.MaxValue);
        var topProducts = await _orderRepository.GetTopProductsByRevenueAsync(10);

        return new SalesReportDto(request.StartDate, request.EndDate, revenue, orders.Count, topProducts);
    }
}

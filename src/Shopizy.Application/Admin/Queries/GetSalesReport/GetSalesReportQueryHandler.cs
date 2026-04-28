using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Admin.Queries.GetSalesReport;

public class GetSalesReportQueryHandler(IOrderRepository orderRepository)
    : IQueryHandler<GetSalesReportQuery, ErrorOr<SalesReportDto>>
{
    private readonly IOrderRepository _orderRepository = orderRepository;

    public async Task<ErrorOr<SalesReportDto>> Handle(
        GetSalesReportQuery request,
        CancellationToken cancellationToken
    )
    {
        var revenue = await _orderRepository.GetRevenueByPeriodAsync(
            request.StartDate,
            request.EndDate
        );
        var orderCount = await _orderRepository.GetOrdersCountByPeriodAsync(
            request.StartDate,
            request.EndDate
        );
        var topProducts = await _orderRepository.GetTopProductsByRevenueAsync(10);

        return new SalesReportDto(
            request.StartDate,
            request.EndDate,
            revenue,
            orderCount,
            topProducts
        );
    }
}

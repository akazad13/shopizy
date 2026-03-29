using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Application.Common.Interfaces.Persistence;

namespace Shopizy.Application.Admin.Queries.GetDashboardMetrics;

public class GetDashboardMetricsQueryHandler(
    IOrderRepository orderRepository,
    IUserRepository userRepository,
    IProductRepository productRepository)
    : IQueryHandler<GetDashboardMetricsQuery, ErrorOr<DashboardMetricsDto>>
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<ErrorOr<DashboardMetricsDto>> Handle(GetDashboardMetricsQuery query, CancellationToken cancellationToken)
    {
        var totalRevenue = await _orderRepository.GetTotalRevenueAsync();
        var totalOrders = await _orderRepository.GetTotalOrdersCountAsync();
        var totalUsers = await _userRepository.GetTotalUsersCountAsync();
        var totalProducts = await _productRepository.GetTotalProductCountAsync();
        
        var lowStockProducts = await _productRepository.GetLowStockProductsAsync(5); // threshold of 5

        var dto = new DashboardMetricsDto(
            totalRevenue,
            totalOrders,
            totalUsers,
            totalProducts,
            lowStockProducts.Select(p => new StockAlertDto(
                p.Id.Value,
                p.Name,
                p.StockQuantity
            )).ToList()
        );

        return dto.ToErrorOr();
    }
}

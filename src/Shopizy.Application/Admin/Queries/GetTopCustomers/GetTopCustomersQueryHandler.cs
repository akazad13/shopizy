using ErrorOr;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.Application.Admin.Queries.GetSalesReport;
using Shopizy.Application.Common.Interfaces.Persistence;

namespace Shopizy.Application.Admin.Queries.GetTopCustomers;

public class GetTopCustomersQueryHandler(IOrderRepository orderRepository)
    : IQueryHandler<GetTopCustomersQuery, ErrorOr<List<TopCustomerDto>>>
{
    private readonly IOrderRepository _orderRepository = orderRepository;

    public async Task<ErrorOr<List<TopCustomerDto>>> Handle(GetTopCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await _orderRepository.GetTopCustomersBySpendAsync(request.Count);
        return customers.ToList();
    }
}

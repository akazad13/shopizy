using Shopizy.Infrastructure.Common.Specifications;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Orders;

namespace Shopizy.Infrastructure.Orders.Specifications;

public class OrderByIdSpec : Specification<Order>
{
    public OrderByIdSpec(OrderId id) : base(order => order.Id == id)
    {
        AddInclude(order => order.OrderItems);
    }
}

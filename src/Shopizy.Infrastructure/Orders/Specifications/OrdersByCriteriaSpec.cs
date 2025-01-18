using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Orders;
using Shopizy.Domain.Orders.Enums;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.Infrastructure.Common.Specifications;

namespace Shopizy.Infrastructure.Orders.Specifications;

internal class OrdersByCriteriaSpec : Specification<Order>
{
    public OrdersByCriteriaSpec(
        UserId? customerId,
        DateTime? startDate,
        DateTime? endDate,
        OrderStatus? status,
        int pageNumber = 1,
        int pageSize = 10,
        OrderType orderType = OrderType.Ascending
    )
    {
        if (customerId is not null)
        {
            AddCriteria(order => order.UserId == customerId);
        }

        if (startDate is not null)
        {
            AddCriteria(order => order.CreatedOn >= startDate);
        }

        if (endDate is not null)
        {
            AddCriteria(order => order.CreatedOn <= endDate);
        }

        if (status is not null)
        {
            AddCriteria(order => order.OrderStatus == status);
        }

        AddPaging(pageSize, pageNumber);
    }
}

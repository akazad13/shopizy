using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Returns.ValueObjects;
using Shopizy.SharedKernel.Domain.Models;

namespace Shopizy.Domain.Returns.Entities;

public sealed class ReturnItem : Entity<ReturnItemId>
{
    public OrderItemId OrderItemId { get; private set; }
    public int Quantity { get; private set; }
    public ReturnRequestId ReturnRequestId { get; private set; } = null!;

    public static ReturnItem Create(OrderItemId orderItemId, int quantity)
    {
        return new ReturnItem(ReturnItemId.CreateUnique(), orderItemId, quantity);
    }

    private ReturnItem(ReturnItemId id, OrderItemId orderItemId, int quantity)
        : base(id)
    {
        OrderItemId = orderItemId;
        Quantity = quantity;
    }

    private ReturnItem() { } // For EF Core
}

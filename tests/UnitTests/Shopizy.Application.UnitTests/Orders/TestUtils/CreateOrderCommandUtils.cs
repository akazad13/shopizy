using Microsoft.AspNetCore.Http.Features;
using Shopizy.Application.Orders.Commands.CreateOrder;
using Shopizy.Application.UnitTests.TestUtils.Constants;

namespace Shopizy.Application.UnitTests.Orders.TestUtils;

public static class CreateOrderCommandUtils
{
    public static CreateOrderCommand CreateCommand(IList<Guid> productIds)
    {
        var orderItems = CreateOrderItemCommand(productIds);
        var addressCommand = new AddressCommand(
            Constants.User.Address.Street,
            Constants.User.Address.City,
            Constants.User.Address.State,
            Constants.User.Address.Country,
            Constants.User.Address.ZipCode
        );
        return new CreateOrderCommand(
            Constants.User.Id.Value,
            Constants.Order.PromoCode,
            Constants.Order.DeliveryCharge.Amount,
            Constants.Order.DeliveryCharge.Currency,
            orderItems,
            addressCommand
        );
    }

    public static IEnumerable<OrderItemCommand> CreateOrderItemCommand(IList<Guid> productIds)
    {
        foreach (var productId in productIds)
        {
            yield return new OrderItemCommand(productId, 1);
        }
    }
}

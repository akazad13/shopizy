using Shopizy.Application.Orders.Commands.CreateOrder;
using Shopizy.Application.UnitTests.TestUtils.Constants;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.UnitTests.Orders.TestUtils;

public static class CreateOrderCommandUtils
{
    public static CreateOrderCommand CreateCommand(ProductId productId)
    {
        var orderItem1 = new OrderItemCommand(productId.Value, 1);
        // var orderItem2 = new OrderItemCommand(Constants.Product.Id.Value, 4);
        var addressCommand = new AddressCommand(Constants.User.Address.Line, Constants.User.Address.City, Constants.User.Address.State, Constants.User.Address.Country, Constants.User.Address.ZipCode);
        return new CreateOrderCommand(Constants.User.Id.Value, Constants.Order.PromoCode, Constants.Order.DeliveryCharge.Amount, Constants.Order.DeliveryCharge.Currency, [orderItem1], addressCommand);
    }
}

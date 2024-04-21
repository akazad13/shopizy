using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.UnitTests.TestUtils.Constants;

public static partial class Constants
{
    public static class Order
    {
        public static readonly OrderId Id = OrderId.Create(
            new Guid("dd0aa32a-f7ab-4d48-b33e-1a3c1092f1e2")
        );
        public const string PromoCode = "WELCOME";
        public const decimal DeliveryChargeAmount = 100;
        public const Currency DeliveryChargeCurrency = 0;
    }

    // public static class OrderItem
    // {
    //     public static readonly ProductId ProductId = ProductId.Create(
    //         new Guid("dd0aa32a-f7ab-4d48-b33e-1a3c1092f1e4")
    //     );
    //     public const int Quantity = 1;
    // }
}

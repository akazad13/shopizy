using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Orders.ValueObjects;

namespace Shopizy.Application.UnitTests.TestUtils.Constants;

public static partial class Constants
{
    public static class Order
    {
        public static readonly OrderId Id = OrderId.Create(
            new Guid("dd0aa32a-f7ab-4d48-b33e-1a3c1092f1e2")
        );
        public const string PromoCode = "WELCOME";
        public const int DeliveryMethod = 1;
        public static readonly Price DeliveryCharge = Price.CreateNew(100, 0);
        public const string CancellationReason = "Cancel Reason";
        public const string Color = "White";
        public const string Size = "XL";
    }
}

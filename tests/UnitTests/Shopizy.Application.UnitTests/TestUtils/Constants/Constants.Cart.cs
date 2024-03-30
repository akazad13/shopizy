using Shopizy.Domain.Carts.ValueObjects;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.UnitTests.TestUtils.Constants;

public static partial class Constants
{
    public static class Cart
    {
        public static readonly CartId Id = CartId.Create(
            new Guid("dd0aa32a-f7ab-4d48-b33e-1a3c1092f1e2")
        );
    }

    public static class LineItem
    {
        public static readonly LineItemId Id = LineItemId.Create(
            new Guid("dd0aa32a-f7ab-4d48-b33e-1a3c1092f1e2")
        );
        public static readonly ProductId ProductId = Product.Id;
        public const int Quantity = 5;
    }
}

using Shopizy.Application.Carts.Queries.GetCart;
using Shopizy.Domain.Carts;

namespace Shopizy.Application.UnitTests.TestUtils.Extensions;

public static partial class CartExtensions
{
    public static void ValidateResult(this Cart cart, GetCartQuery query)
    {
        Assert.Equal(query.UserId, cart.UserId.Value);
        Assert.Single(cart.CartItems);
    }
}

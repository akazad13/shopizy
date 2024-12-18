using FluentAssertions;
using Shopizy.Application.Carts.Queries.GetCart;
using Shopizy.Domain.Carts;

namespace Shopizy.Application.UnitTests.TestUtils.Extensions;

public static partial class CartExtensions
{
    public static void ValidateResult(this Cart cart, GetCartQuery query)
    {
        cart.UserId.Value.Should().Be(query.UserId);
        cart.CartItems.Should().HaveCount(1);
    }
}

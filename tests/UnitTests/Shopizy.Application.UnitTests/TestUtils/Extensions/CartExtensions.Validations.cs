using FluentAssertions;
using Shopizy.Application.Carts.Queries.GetCart;
using Shopizy.Domain.Carts;

namespace Shopizy.Application.UnitTests.TestUtils.Extensions;

public static partial class CartExtensions
{
    public static void ValidateResult(this Cart cart, GetCartQuery query)
    {
        cart.CustomerId.Value.Should().Be(query.CustomerId);
        cart.LineItems.Should().HaveCount(1);
        cart.ModifiedOn.Should().NotBeBefore(cart.CreatedOn);
    }
}

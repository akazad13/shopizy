using FluentAssertions;
using Moq;
using Shopizy.Application.Carts.Queries.GetCart;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Application.UnitTests.Carts.TestUtils;
using Shopizy.Application.UnitTests.TestUtils.Extensions;
using Shopizy.Domain.Customers.ValueObjects;

namespace Shopizy.Application.UnitTests.Carts.Queries.GetCart;

public class GetCartQueryHandlerTests
{
    private readonly GetCartQueryHandler _handler;
    private readonly Mock<ICartRepository> _mockCartRepository;

    public GetCartQueryHandlerTests()
    {
        _mockCartRepository = new Mock<ICartRepository>();
        _handler = new GetCartQueryHandler(_mockCartRepository.Object);
    }

    [Fact]
    public async void GetCart_WhenProductIsFound_ShouldReturnProduct()
    {
        // Arrange
        var cart = CartFactory.Create();
        cart.AddLineItem(CartFactory.CreateLineItem());

        var query = GetCartQueryUtils.CreateQuery();

        _mockCartRepository
            .Setup(c => c.GetCartByCustomerIdAsync(CustomerId.Create(query.CustomerId)))
            .ReturnsAsync(cart);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value?.ValidateResult(query);
    }
}

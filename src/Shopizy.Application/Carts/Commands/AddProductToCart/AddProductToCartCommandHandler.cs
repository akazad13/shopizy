using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Security.CurrentUser;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Carts.Entities;
using Shopizy.Domain.Carts.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Carts.Commands.AddProductToCart;

public class AddProductToCartCommandHandler(
    ICartRepository cartRepository,
    IProductRepository productRepository,
    ICurrentUser currentUser
) : IRequestHandler<AddProductToCartCommand, ErrorOr<Cart>>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly ICartRepository _cartRepository = cartRepository;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<ErrorOr<Cart>> Handle(
        AddProductToCartCommand cmd,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(cmd, nameof(cmd));

        var cartId = CartId.Create(cmd.CartId);
        var productId = ProductId.Create(cmd.ProductId);

        var cart = await _cartRepository.GetCartByIdAsync(cartId, cancellationToken);

        if (cart is null)
        {
            return CustomErrors.Cart.CartNotFound;
        }

        if (cart.CartItems.Any(li => li.ProductId == productId && li.Color == cmd.Color && li.Size == cmd.Size))
        {
            return CustomErrors.Cart.ProductAlreadyExistInCart;
        }

        if (!await _productRepository.IsProductExistAsync(productId))
        {
            return CustomErrors.Product.ProductNotFound;
        }

        cart.AddLineItem(CartItem.Create(productId, cmd.Color, cmd.Size, cmd.Quantity));
        _cartRepository.Update(cart);

        if (await _cartRepository.Commit(cancellationToken) <= 0)
        {
            return CustomErrors.Cart.CartPrductNotAdded;
        }

        return await _cartRepository.GetCartByUserIdAsync(UserId.Create(_currentUser.GetCurrentUserId()));
    }
}

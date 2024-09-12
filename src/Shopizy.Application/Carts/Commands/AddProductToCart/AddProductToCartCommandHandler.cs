using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Carts.Entities;
using Shopizy.Domain.Carts.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Carts.Commands.AddProductToCart;

public class AddProductToCartCommandHandler(ICartRepository cartRepository, IProductRepository productRepository)
        : IRequestHandler<AddProductToCartCommand, ErrorOr<Cart>>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly ICartRepository _cartRepository = cartRepository;

    public async Task<ErrorOr<Cart>> Handle(AddProductToCartCommand cmd, CancellationToken cancellationToken)
    {
        Cart? cart = await _cartRepository.GetCartByIdAsync(CartId.Create(cmd.CartId));

        if (cart is null)
        {
            return CustomErrors.Cart.CartNotFound;
        }

        if (cart.LineItems.Any(li => li.ProductId == ProductId.Create(cmd.ProductId)))
        {
            return CustomErrors.Cart.ProductAlreadyExistInCart;
        }

        bool product = await _productRepository.IsProductExistAsync(ProductId.Create(cmd.ProductId));

        if (!product)
        {
            return CustomErrors.Product.ProductNotFound;
        }

        cart.AddLineItem(LineItem.Create(ProductId.Create(cmd.ProductId)));

        _cartRepository.Update(cart);

        if (await _cartRepository.Commit(cancellationToken) <= 0)
        {
            return CustomErrors.Cart.CartPrductNotAdded;
        }

        return (await _cartRepository.GetCartByUserIdAsync(UserId.Create(cmd.UserId)))!;
    }
}

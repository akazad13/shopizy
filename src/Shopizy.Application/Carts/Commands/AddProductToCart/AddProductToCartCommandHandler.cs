using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Carts.Entities;
using Shopizy.Domain.Carts.ValueObjects;

namespace Shopizy.Application.Carts.Commands.AddProductToCart;

public class AddProductToCartCommandHandler(ICartRepository _cartRepository, IProductRepository _productRepository)
        : IRequestHandler<AddProductToCartCommand, ErrorOr<Cart>>
{
    public async Task<ErrorOr<Cart>> Handle(AddProductToCartCommand cmd, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetCartByIdAsync(CartId.Create(cmd.CartId));

        if(cart is null)
            return CustomErrors.Cart.CartNotFound;

        var product = await _productRepository.IsProductExistAsync(ProductId.Create(cmd.ProductId));

        if(product is false)
            return CustomErrors.Product.ProductNotFound;
        
        cart.AddLineItem(LineItem.Create(ProductId.Create(cmd.ProductId)));

        _cartRepository.Update(cart);
        
        if (await _cartRepository.Commit(cancellationToken) <= 0)
            return CustomErrors.Cart.CartPrductNotAdded;
        return cart;

    }
}
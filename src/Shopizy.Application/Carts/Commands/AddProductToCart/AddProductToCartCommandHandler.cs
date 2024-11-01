using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Carts.Entities;
using Shopizy.Domain.Carts.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Carts.Commands.AddProductToCart;

public class AddProductToCartCommandHandler(
    ICartRepository cartRepository,
    IProductRepository productRepository
) : IRequestHandler<AddProductToCartCommand, IResult<Cart>>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly ICartRepository _cartRepository = cartRepository;

    public async Task<IResult<Cart>> Handle(
        AddProductToCartCommand cmd,
        CancellationToken cancellationToken
    )
    {
        var cart = await _cartRepository.GetCartByIdAsync(
            CartId.Create(cmd.CartId),
            cancellationToken
        );

        if (cart is null)
        {
            return Response<Cart>.ErrorResponse([CustomErrors.Cart.CartNotFound]);
        }

        if (cart.LineItems.Any(li => li.ProductId == ProductId.Create(cmd.ProductId)))
        {
            return Response<Cart>.ErrorResponse([CustomErrors.Cart.ProductAlreadyExistInCart]);
        }

        var product = await _productRepository.IsProductExistAsync(ProductId.Create(cmd.ProductId));

        if (!product)
        {
            return Response<Cart>.ErrorResponse([CustomErrors.Product.ProductNotFound]);
        }

        cart.AddLineItem(LineItem.Create(ProductId.Create(cmd.ProductId)));

        _cartRepository.Update(cart);

        if (await _cartRepository.Commit(cancellationToken) <= 0)
        {
            return Response<Cart>.ErrorResponse([CustomErrors.Cart.CartPrductNotAdded]);
        }

        return Response<Cart>.SuccessResponese(
            await _cartRepository.GetCartByUserIdAsync(UserId.Create(cmd.UserId))
        );
    }
}

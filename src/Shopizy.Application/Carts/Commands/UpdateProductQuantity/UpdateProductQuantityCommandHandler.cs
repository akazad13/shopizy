using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Domain.Carts.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.Carts.Commands.UpdateProductQuantity;

public class UpdateProductQuantityCommandHandler(ICartRepository cartRepository)
    : IRequestHandler<UpdateProductQuantityCommand, IResult<GenericResponse>>
{
    private readonly ICartRepository _cartRepository = cartRepository;

    public async Task<IResult<GenericResponse>> Handle(
        UpdateProductQuantityCommand cmd,
        CancellationToken cancellationToken
    )
    {
        var cart = await _cartRepository.GetCartByIdAsync(
            CartId.Create(cmd.CartId),
            cancellationToken
        );

        if (cart is null)
        {
            return Response<GenericResponse>.ErrorResponse([CustomErrors.Cart.CartNotFound]);
        }

        cart.UpdateLineItem(ProductId.Create(cmd.ProductId), cmd.Quantity);

        _cartRepository.Update(cart);

        if (await _cartRepository.Commit(cancellationToken) <= 0)
        {
            return Response<GenericResponse>.ErrorResponse([CustomErrors.Cart.CartPrductNotAdded]);
        }

        return Response<GenericResponse>.SuccessResponese("successfully updated cart.");
    }
}

using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Wrappers;
using Shopizy.Domain.Carts.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;

namespace Shopizy.Application.Carts.Commands.RemoveProductFromCart;

public class RemoveProductFromCartCommandHandler(ICartRepository cartRepository)
    : IRequestHandler<RemoveProductFromCartCommand, IResult<GenericResponse>>
{
    private readonly ICartRepository _cartRepository = cartRepository;

    public async Task<IResult<GenericResponse>> Handle(
        RemoveProductFromCartCommand cmd,
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

        var lineItem = cart.LineItems.FirstOrDefault(li => li.ProductId.Value == cmd.ProductId);
        if (lineItem is not null)
        {
            cart.RemoveLineItem(lineItem);
        }

        _cartRepository.Update(cart);

        if (await _cartRepository.Commit(cancellationToken) <= 0)
        {
            return Response<GenericResponse>.ErrorResponse(
                [CustomErrors.Cart.CartPrductNotRemoved]
            );
        }

        return Response<GenericResponse>.SuccessResponese(
            "successfully removed product from cart."
        );
    }
}

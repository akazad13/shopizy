using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Carts.ValueObjects;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.Carts.Commands.UpdateProductQuantity;

public class UpdateProductQuantityCommandHandler(ICartRepository cartRepository)
        : IRequestHandler<UpdateProductQuantityCommand, ErrorOr<Success>>
{
    private readonly ICartRepository _cartRepository = cartRepository;
    public async Task<ErrorOr<Success>> Handle(UpdateProductQuantityCommand cmd, CancellationToken cancellationToken)
    {
        Domain.Carts.Cart? cart = await _cartRepository.GetCartByIdAsync(CartId.Create(cmd.CartId));

        if (cart is null)
        {
            return CustomErrors.Cart.CartNotFound;
        }

        cart.UpdateLineItem(ProductId.Create(cmd.ProductId), cmd.Quantity);

        _cartRepository.Update(cart);

        if (await _cartRepository.Commit(cancellationToken) <= 0)
        {
            return CustomErrors.Cart.CartPrductNotAdded;
        }

        return Result.Success;

    }
}

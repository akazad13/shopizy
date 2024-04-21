using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Carts.Entities;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Carts.Commands.CreateCartWithFirstProduct;

public class CreateCartWithFirstProductCommandHandler(IProductRepository productRepository, ICartRepository cartRepository)
        : IRequestHandler<CreateCartWithFirstProductCommand, ErrorOr<Cart>>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly ICartRepository _cartRepository = cartRepository;
    public async Task<ErrorOr<Cart>> Handle(CreateCartWithFirstProductCommand cmd, CancellationToken cancellationToken)
    {
        var product = await _productRepository.IsProductExistAsync(ProductId.Create(cmd.ProductId));

        if(!product)
            return CustomErrors.Product.ProductNotFound;
        
        var cart = Cart.Create(UserId.Create(cmd.UserId));
        cart.AddLineItem(LineItem.Create(ProductId.Create(cmd.ProductId)));

        await _cartRepository.AddAsync(cart);
        
        if (await _cartRepository.Commit(cancellationToken) <= 0)
            return CustomErrors.Cart.CartNotCreated;
        return (await _cartRepository.GetCartByUserIdAsync(UserId.Create(cmd.UserId)))!;
    }
}
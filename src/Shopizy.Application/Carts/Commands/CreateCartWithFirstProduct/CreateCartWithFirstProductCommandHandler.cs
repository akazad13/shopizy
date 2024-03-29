using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Domain.Carts;
using Shopizy.Domain.Carts.Entities;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Customers.ValueObjects;
using Shopizy.Domain.Products.ValueObjects;

namespace Shopizy.Application.Carts.Commands.CreateCartWithFirstProduct;

public class CreateCartWIthFirstProductCommandHandler(IProductRepository _productRepository, ICartRepository _cartRepository)
        : IRequestHandler<CreateCartWithFirstProductCommand, ErrorOr<Cart>>
{
    public async Task<ErrorOr<Cart>> Handle(CreateCartWithFirstProductCommand cmd, CancellationToken cancellationToken)
    {
        var product = await _productRepository.IsProductExistAsync(ProductId.Create(cmd.ProductId));

        if(product is false)
            return CustomErrors.Product.ProductNotFound;
        
        var cart = Cart.Create(CustomerId.Create(cmd.CustomerId));
        cart.AddLineItem(LineItem.Create(ProductId.Create(cmd.ProductId), cmd.Quantity));

        await _cartRepository.AddAsync(cart);
        
        if (await _cartRepository.Commit(cancellationToken) <= 0)
            return CustomErrors.Cart.CartNotCreated;
        return cart;

    }
}
using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Security.CurrentUser;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Orders;
using Shopizy.Domain.Orders.Entities;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler(
    IProductRepository productRepository,
    IOrderRepository orderRepository,
    ICurrentUser currentUser
) : IRequestHandler<CreateOrderCommand, ErrorOr<Order>>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<ErrorOr<Order>> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken
    )
    {
        var products = await _productRepository.GetProductsByIdsAsync(
            request.OrderItems.Select(x => ProductId.Create(x.ProductId)).ToList()
        );
        if (products.Count == 0)
        {
            return CustomErrors.Product.ProductNotFound;
        }

        // foreach( var product in products)
        // {
        //     if(product.StockQuantity < request.OrderItems.First(p => p.ProductId == items.Id.Value).Quantity)
        //     {
        //         return "Prduct is not available";
        //     }
        // }

        var order = Order.Create(
            userId: UserId.Create(_currentUser.GetCurrentUserId()),
            promoCode: request.PromoCode,
            deliveryMethod: request.DeliveryMethod,
            deliveryCharge: Price.CreateNew(
                request.DeliveryChargeAmount,
                request.DeliveryChargeCurrency
            ),
            shippingAddress: Address.CreateNew(
                street: request.ShippingAddress.Street,
                city: request.ShippingAddress.City,
                state: request.ShippingAddress.State,
                country: request.ShippingAddress.Country,
                zipCode: request.ShippingAddress.ZipCode
            ),
            orderItems: products.ConvertAll(product =>
            {
                var item = request.OrderItems.First(p => p.ProductId == product.Id.Value);
                var photoUrl =
                    product.ProductImages.Count == 0 ? "" : product.ProductImages[0].ImageUrl;

                return OrderItem.Create(
                    name: product.Name,
                    pictureUrl: photoUrl,
                    unitPrice: product.UnitPrice,
                    quantity: item.Quantity,
                    color: item.Color,
                    size: item.Size,
                    discount: product.Discount
                );
            })
        );

        await _orderRepository.AddAsync(order);
        if (await _orderRepository.Commit(cancellationToken) <= 0)
        {
            return CustomErrors.Order.OrderNotCreated;
        }
        return order;
    }
}

using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Wrappers;
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
    IOrderRepository orderRepository
) : IRequestHandler<CreateOrderCommand, IResult<Order>>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IOrderRepository _orderRepository = orderRepository;

    public async Task<IResult<Order>> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken
    )
    {
        var products = await _productRepository.GetProductsByIdsAsync(
            request.OrderItems.Select(x => ProductId.Create(x.ProductId)).ToList()
        );
        if (products.Count == 0)
        {
            return Response<Order>.ErrorResponse([CustomErrors.Product.ProductNotFound]);
        }

        // foreach( var product in products)
        // {
        //     if(product.StockQuantity < request.OrderItems.First(p => p.ProductId == items.Id.Value).Quantity)
        //     {
        //         return "Prduct is not available";
        //     }
        // }

        var order = Order.Create(
            userId: UserId.Create(request.UserId),
            promoCode: request.PromoCode,
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
            orderItems: products.ConvertAll(items =>
                OrderItem.Create(
                    name: items.Name,
                    pictureUrl: items.ProductImages.Count == 0
                        ? ""
                        : items.ProductImages[0].ImageUrl,
                    unitPrice: items.UnitPrice,
                    quantity: request.OrderItems.First(p => p.ProductId == items.Id.Value).Quantity,
                    discount: items.Discount
                )
            )
        );

        await _orderRepository.AddAsync(order);
        if (await _orderRepository.Commit(cancellationToken) <= 0)
        {
            return Response<Order>.ErrorResponse([CustomErrors.Order.OrderNotCreated]);
        }
        return Response<Order>.SuccessResponese(order);
    }
}

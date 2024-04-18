using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Orders;
using Shopizy.Domain.Orders.Entities;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler(IProductRepository productRepository, IOrderRepository orderRepository) : IRequestHandler<CreateOrderCommand, ErrorOr<Order>>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IOrderRepository _orderRepository = orderRepository;

    public async Task<ErrorOr<Order>> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken
    )
    {
        var products = await _productRepository.GetProductsByIdsAsync( request.OrderItems.Select( x => ProductId.Create( x.ProductId )));

        var order = Order.Create(
            userId: UserId.Create(request.UserId),
            promoCode: request.PromoCode,
            deliveryCharge: request.DeliveryCharge,
            shippingAddress: Address.CreateNew(
                line: request.Address.Line,
                city: request.Address.City,
                state: request.Address.State,
                country: request.Address.Country,
                zipCode: request.Address.ZipCode
            ),
            orderItems: products.ConvertAll( items => OrderItem.Create(
                name: items.Name,
                pictureUrl: items.ProductImages[0]!.ImageUrl,
                unitPrice: items.UnitPrice,
                quantity: request.OrderItems.First(p => p.ProductId == items.Id.Value).Quantity,
                discount: items.Discount
            ) )
        );

        await _orderRepository.AddAsync(order);
        if (await _orderRepository.Commit(cancellationToken) <= 0)
            return CustomErrors.Order.OrderNotCreated;

        return order;
    }
}

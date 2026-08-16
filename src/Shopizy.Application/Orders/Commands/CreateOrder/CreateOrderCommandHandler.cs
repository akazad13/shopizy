using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Orders;
using Shopizy.Domain.Orders.Entities;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Products.ValueObjects;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Application.Interfaces.Persistence;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler(
    IProductRepository productRepository,
    IOrderRepository orderRepository,
    IGiftCardRepository giftCardRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateOrderCommand, ErrorOr<Order>>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IGiftCardRepository _giftCardRepository = giftCardRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ErrorOr<Order>> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken
    )
    {
        var products = await _productRepository.GetProductsByIdsAsync(
            request.OrderItems.Select(x => ProductId.Create(x.ProductId)).ToList()
        );
        if (products == null || products.Count == 0)
        {
            return (Error)CustomErrors.Product.ProductNotFound;
        }

        foreach (var product in products)
        {
            var requestedItem = request.OrderItems.FirstOrDefault(i =>
                i.ProductId == product.Id.Value
            );
            if (requestedItem != null && product.StockQuantity < requestedItem.Quantity)
            {
                return (Error)CustomErrors.Product.InsufficientStock;
            }
        }

        var order = Order.Create(
            userId: UserId.Create(request.UserId),
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
            orderItems: request
                .OrderItems.ToList()
                .ConvertAll(item =>
                {
                    var product = products.First(p => p.Id.Value == item.ProductId);
                    var photoUrl = product.ProductImages?.FirstOrDefault()?.ImageUrl ?? "";

                    return OrderItem.Create(
                        productId: product.Id,
                        name: product.Name,
                        pictureUrl: photoUrl,
                        unitPrice: Price.CreateNew(
                            product.UnitPrice.Amount,
                            product.UnitPrice.Currency
                        ),
                        quantity: item.Quantity,
                        color: item.Color,
                        size: item.Size,
                        discount: product.Discount
                    );
                })
        );

        if (!string.IsNullOrWhiteSpace(request.GiftCardCode))
        {
            var giftCard = await _giftCardRepository.GetByCodeAsync(request.GiftCardCode);
            if (giftCard is null)
            {
                return (Error)CustomErrors.GiftCard.GiftCardNotFound;
            }

            var currentTotal = order.GetTotal().Amount;
            if (currentTotal > 0 && giftCard.RemainingBalance > 0)
            {
                var amountToApply = Math.Min(currentTotal, giftCard.RemainingBalance);
                var applyResult = giftCard.ApplyToOrder(amountToApply);

                if (applyResult.IsError)
                {
                    return applyResult.Error.ToError();
                }

                order = Order.Create(
                    userId: order.UserId,
                    promoCode: order.PromoCode,
                    deliveryMethod: (int)order.DeliveryMethod,
                    deliveryCharge: order.DeliveryCharge,
                    shippingAddress: order.ShippingAddress,
                    orderItems: order.OrderItems,
                    giftCardId: giftCard.Id,
                    giftCardAmountApplied: amountToApply
                );

                _giftCardRepository.Update(giftCard);
            }
        }

        await _orderRepository.AddAsync(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return order;
    }
}

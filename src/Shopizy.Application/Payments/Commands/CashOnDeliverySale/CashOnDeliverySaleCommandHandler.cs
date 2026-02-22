using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Common.Enums;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Orders.Enums;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Payments;
using Shopizy.Domain.Payments.Enums;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Payments.Commands.CashOnDeliverySale;

public class CashOnDeliverySaleCommandHandler(
    IPaymentRepository paymentRepository,
    IOrderRepository orderRepository
) : IRequestHandler<CashOnDeliverySaleCommand, ErrorOr<Success>>
{
    private readonly IPaymentRepository _paymentRepository = paymentRepository;
    private readonly IOrderRepository _orderRepository = orderRepository;

    public async Task<ErrorOr<Success>> Handle(
        CashOnDeliverySaleCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var order = await _orderRepository.GetOrderByIdAsync(OrderId.Create(request.OrderId));

            if (order is null)
            {
                return CustomErrors.Order.OrderNotFound;
            }

            var payment = Payment.Create(
                UserId.Create(request.UserId),
                OrderId.Create(request.OrderId),
                request.PaymentMethod,
                "",
                "",
                PaymentStatus.Pending,
                Price.CreateNew(request.Amount, Currency.usd),
                order.ShippingAddress
            );

            await _paymentRepository.AddAsync(payment);

            order.UpdateOrderStatus(OrderStatus.Processing);

            _orderRepository.Update(order);

            return Result.Success;
        }
        catch (Exception)
        {
            return Error.Failure(
                code: "payment.failed",
                description: "Failed to collect the payment."
            );
        }
    }
}

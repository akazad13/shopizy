using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Common.models;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Payments;
using Shopizy.Domain.Payments.Enums;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Payments.Commands.CreatePayment;

public class CreateOrderCommandHandler(
    IPaymentRepository paymentRepository,
    IOrderRepository orderRepository,
    IPaymentService paymentService
) : IRequestHandler<CreatePaymentCommand, ErrorOr<CheckoutSession>>
{
    private readonly IPaymentRepository _paymentRepository = paymentRepository;
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IPaymentService _paymentService = paymentService;

    public async Task<ErrorOr<CheckoutSession>> Handle(
        CreatePaymentCommand request,
        CancellationToken cancellationToken
    )
    {
        var order = await _orderRepository.GetOrderByIdAsync(OrderId.Create(request.OrderId));

        if (order is null)
        {
            return CustomErrors.Order.OrderNotFound;
        }

        var total = order.GetTotal();

        var payment = Payment.Create(
            UserId.Create(request.UserId),
            OrderId.Create(request.OrderId),
            "",
            "",
            PaymentStatus.Pending,
            total,
            order.ShippingAddress
        );

        await _paymentRepository.AddAsync(payment);

        if (await _paymentRepository.Commit(cancellationToken) <= 0)
        {
            return CustomErrors.Payment.PaymentNotCreated;
        }

        var successUrl = "http://localhost:5054/success";
        var cancelUrl = "http://localhost:5054/cancel";

        var checkoutSession = await _paymentService.CreateCheckoutSession(
            "",
            total.Amount,
            successUrl,
            cancelUrl,
            cancellationToken
        );

        return checkoutSession;
    }
}

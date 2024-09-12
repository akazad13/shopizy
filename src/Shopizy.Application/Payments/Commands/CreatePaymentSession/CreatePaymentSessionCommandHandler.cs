using ErrorOr;
using MediatR;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Application.Common.models;
using Shopizy.Application.Payments.Commands.CreatePayment;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Payments;
using Shopizy.Domain.Payments.Enums;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Application.Payments.Commands.CreatePaymentSession;

public class CreatePaymentSessionCommandHandler(
    IPaymentRepository paymentRepository,
    IOrderRepository orderRepository,
    IPaymentService paymentService
) : IRequestHandler<CreatePaymentSessionCommand, ErrorOr<CheckoutSession>>
{
    private readonly IPaymentRepository _paymentRepository = paymentRepository;
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IPaymentService _paymentService = paymentService;

    public async Task<ErrorOr<CheckoutSession>> Handle(
        CreatePaymentSessionCommand request,
        CancellationToken cancellationToken
    )
    {
        Domain.Orders.Order? order = await _orderRepository.GetOrderByIdAsync(
            OrderId.Create(request.OrderId)
        );

        if (order is null)
        {
            return CustomErrors.Order.OrderNotFound;
        }

        Domain.Common.ValueObjects.Price total = order.GetTotal();

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

        string successUrl = "http://localhost:4200/success";
        string cancelUrl = "http://localhost:4200/cancel";

        ErrorOr<CheckoutSession> checkoutSession = await _paymentService.CreateCheckoutSession(
            "",
            total.Amount,
            successUrl,
            cancelUrl,
            cancellationToken
        );

        return checkoutSession;
    }
}

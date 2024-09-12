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
) : IRequestHandler<CreatePaymentCommand, ErrorOr<ChargeResource>>
{
    private readonly IPaymentRepository _paymentRepository = paymentRepository;
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IPaymentService _paymentService = paymentService;

    public async Task<ErrorOr<ChargeResource>> Handle(
        CreatePaymentCommand request,
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

        // await _paymentRepository.AddAsync(payment);

        // if (await _paymentRepository.Commit(cancellationToken) <= 0)
        // {
        //     return CustomErrors.Payment.PaymentNotCreated;
        // }

        var customerResource = await _paymentService.CreateCustomer(
            email: "john@example.com",
            name: "Kalam",
            cancellationToken
        );

        var createChargeResource = await _paymentService.CreateCharge(
            currency: "usd",
            (long)(total.Amount * 100),
            customerResource.Value.Email,
            customerResource.Value.CustomerId,
            description: "Test charge",
            cancellationToken: cancellationToken
        );

        return createChargeResource;
    }
}

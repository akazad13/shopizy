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

namespace Shopizy.Application.Payments.Commands.CreatePaymentSession;

public class CreatePaymentSessionCommandHandler(
    IPaymentRepository paymentRepository,
    IOrderRepository orderRepository,
    IUserRepository userRepository,
    IPaymentService paymentService
) : IRequestHandler<CreatePaymentSessionCommand, ErrorOr<CheckoutSession>>
{
    private readonly IPaymentRepository _paymentRepository = paymentRepository;
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPaymentService _paymentService = paymentService;

    public async Task<ErrorOr<CheckoutSession>> Handle(
        CreatePaymentSessionCommand request,
        CancellationToken cancellationToken
    )
    {
        var order = await _orderRepository.GetOrderByIdAsync(OrderId.Create(request.OrderId));

        if (order is null)
        {
            return CustomErrors.Order.OrderNotFound;
        }

        var total = order.GetTotal();

        var user = await _userRepository.GetUserById(UserId.Create(request.UserId));

        var payment = Payment.Create(
            UserId.Create(request.UserId),
            OrderId.Create(request.OrderId),
            request.PaymentType,
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

        return await _paymentService.CreateCheckoutSession(
            user?.Email ?? "",
            total.Amount,
            request.SuccessUrl,
            request.CancelUrl,
            cancellationToken
        );
    }
}

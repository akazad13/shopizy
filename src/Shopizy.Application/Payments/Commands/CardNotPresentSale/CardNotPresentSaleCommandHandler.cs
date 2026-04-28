using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Payments;
using Shopizy.Domain.Payments.Enums;
using Shopizy.Domain.Users.ValueObjects;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.SharedKernel.Application.Models;

namespace Shopizy.Application.Payments.Commands.CardNotPresentSale;

public class CardNotPresentSaleCommandHandler(
    IPaymentRepository paymentRepository,
    IOrderRepository orderRepository,
    IUserRepository userRepository,
    IPaymentService paymentService
) : ICommandHandler<CardNotPresentSaleCommand, ErrorOr<Success>>
{
    private readonly IPaymentRepository _paymentRepository = paymentRepository;
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPaymentService _paymentService = paymentService;

    public async Task<ErrorOr<Success>> Handle(
        CardNotPresentSaleCommand request,
        CancellationToken cancellationToken
    )
    {
        var order = await _orderRepository.GetOrderByIdAsync(OrderId.Create(request.OrderId));

        if (order is null)
        {
            return (Error)CustomErrors.Order.OrderNotFound;
        }

        var total = order.GetTotal();

        var user = await _userRepository.GetUserByIdAsync(UserId.Create(request.UserId));

        if (user is null)
        {
            return (Error)CustomErrors.User.UserNotFound;
        }

        var payment = Payment.Create(
            UserId.Create(request.UserId),
            OrderId.Create(request.OrderId),
            request.PaymentMethod,
            request.PaymentMethodId,
            "",
            PaymentStatus.Pending,
            total,
            order.ShippingAddress
        );

        await _paymentRepository.AddAsync(payment);

        // Create customer id if not present already
        if (user.CustomerId == null)
        {
            var customer = await _paymentService.CreateCustomer(
                user.Email,
                $"{user.FirstName} {user.LastName}",
                cancellationToken
            );

            if (customer.IsError)
            {
                return (Error)CustomErrors.Payment.CustomerNotCreated;
            }

            user.UpdateCustomerId(customer.Value.CustomerId);
        }

        var req = new CreateSaleRequest()
        {
            CustomerId = user.CustomerId,
            PaymentMethodId = request.PaymentMethodId,
            Amount = (long)(total.Amount * 100),
            Currency = request.Currency,
            PaymentMethodTypes = ["card"],
            CapturePayment = true,
        };

        req.SetMetadata(new Metadata() { OrderId = order.Id.Value.ToString() });
        var response = await _paymentService.CreateSaleAsync(req);

        if (response.IsError)
        {
            return Error.Failure(
                code: "payment.failed",
                description: "Failed to collect the payment."
            );
        }

        payment.Complete(response.Value.ChargeId, user.CustomerId!);

        return Result.Success;
    }
}

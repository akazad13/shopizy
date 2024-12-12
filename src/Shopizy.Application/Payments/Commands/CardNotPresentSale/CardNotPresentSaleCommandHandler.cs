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

namespace Shopizy.Application.Payments.Commands.CardNotPresentSale;

public class CardNotPresentSaleCommandHandler(
    IPaymentRepository paymentRepository,
    IOrderRepository orderRepository,
    IUserRepository userRepository,
    IPaymentService paymentService
) : IRequestHandler<CardNotPresentSaleCommand, ErrorOr<Success>>
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
        try
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
                request.PaymentMethod,
                request.PaymentMethodId,
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
                    return CustomErrors.Payment.CustomerNotCreated;
                }

                user.UpdateCustomerId(customer.Value.CustomerId);

                _userRepository.Update(user);
                await _userRepository.Commit(cancellationToken);
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

            order.UpdatePaymentStatus(PaymentStatus.Payed);
            payment.UpdatePaymentStatus(PaymentStatus.Payed);
            payment.UpdateTransactionId(response.Value.ChargeId);

            _orderRepository.Update(order);
            _paymentRepository.Update(payment);
            await _orderRepository.Commit(cancellationToken);

            return Result.Success;
        }
        catch (Exception ex)
        {
            return Error.Failure(
                code: "payment.failed",
                description: "Failed to collect the payment."
            );
        }
    }
}

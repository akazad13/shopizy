using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Domain.Orders.Events;
using Shopizy.Domain.Payments.Enums;
using Shopizy.SharedKernel.Application.Interfaces.Persistence;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Orders.Events;

public class OrderCancelledDomainEventHandler(
    IPaymentRepository paymentRepository,
    IPaymentService paymentService,
    IUnitOfWork unitOfWork
) : IDomainEventHandler<OrderCancelledDomainEvent>
{
    public async Task Handle(
        OrderCancelledDomainEvent domainEvent,
        CancellationToken cancellationToken = default
    )
    {
        var payment = await paymentRepository.GetPaymentByOrderIdAsync(domainEvent.Order.Id);

        if (payment is null || payment.PaymentStatus != PaymentStatus.Payed)
        {
            return;
        }

        if (string.IsNullOrEmpty(payment.TransactionId))
        {
            return;
        }

        var refundResult = await paymentService.CreateRefundAsync(payment.TransactionId, cancellationToken);

        if (refundResult.IsError)
        {
            return;
        }

        payment.UpdatePaymentStatus(PaymentStatus.Refunded);
        paymentRepository.Update(payment);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

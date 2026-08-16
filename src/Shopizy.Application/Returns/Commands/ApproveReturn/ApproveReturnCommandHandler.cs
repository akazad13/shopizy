using ErrorOr;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Domain.Common.CustomErrors;
using Shopizy.Domain.Orders.Enums;
using Shopizy.Domain.Payments.Enums;
using Shopizy.Domain.Returns.ValueObjects;
using Shopizy.SharedKernel.Application.Interfaces.Persistence;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.SharedKernel.Application.Models;

namespace Shopizy.Application.Returns.Commands.ApproveReturn;

public class ApproveReturnCommandHandler(
    IReturnRequestRepository returnRequestRepository,
    IPaymentRepository paymentRepository,
    IOrderRepository orderRepository,
    IPaymentService paymentService,
    IUnitOfWork unitOfWork
) : ICommandHandler<ApproveReturnCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        ApproveReturnCommand request,
        CancellationToken cancellationToken
    )
    {
        var returnId = ReturnRequestId.Create(request.ReturnRequestId);
        var returnRequest = await returnRequestRepository.GetByIdAsync(returnId, cancellationToken);

        if (returnRequest is null)
        {
            return (Error)CustomErrors.ReturnRequest.ReturnNotFound;
        }

        var approveResult = returnRequest.Approve();
        if (approveResult.IsError)
        {
            return approveResult.Error.ToError();
        }

        var payment = await paymentRepository.GetPaymentByOrderIdAsync(returnRequest.OrderId);
        if (
            payment is null
            || string.IsNullOrEmpty(payment.TransactionId)
            || payment.PaymentStatus != PaymentStatus.Payed
        )
        {
            // Payment not valid for refund
            return Error.Validation(
                "Return.PaymentInvalid",
                "Associated payment is invalid for refund."
            );
        }

        var refundResult = await paymentService.CreateRefundAsync(
            payment.TransactionId,
            cancellationToken
        );
        if (refundResult.IsError)
        {
            return refundResult.Errors;
        }

        payment.UpdatePaymentStatus(PaymentStatus.Refunded);
        paymentRepository.Update(payment);

        var completeRefundResult = returnRequest.CompleteRefund();
        if (completeRefundResult.IsError)
        {
            return completeRefundResult.Error.ToError();
        }

        var order = await orderRepository.GetOrderByIdAsync(returnRequest.OrderId);
        if (order is not null)
        {
            order.UpdateOrderStatus(OrderStatus.Refunded);
            orderRepository.Update(order);
        }

        returnRequestRepository.Update(returnRequest);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}

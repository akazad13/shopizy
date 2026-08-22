using ErrorOr;
using Microsoft.Extensions.Logging;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Domain.Orders.Enums;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Payments.Enums;
using Shopizy.SharedKernel.Application.Interfaces.Persistence;
using Shopizy.SharedKernel.Application.Messaging;
using Shopizy.SharedKernel.Application.Models;

namespace Shopizy.Application.Payments.Commands.ProcessStripeWebhook;

/// <summary>
/// Handles processing of incoming Stripe webhook events with idempotent side-effects.
/// </summary>
public sealed class ProcessStripeWebhookCommandHandler(
    IPaymentService paymentService,
    IPaymentRepository paymentRepository,
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    ILogger<ProcessStripeWebhookCommandHandler> logger
) : ICommandHandler<ProcessStripeWebhookCommand, ErrorOr<Success>>
{
    private readonly IPaymentService _paymentService = paymentService;
    private readonly IPaymentRepository _paymentRepository = paymentRepository;
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<ProcessStripeWebhookCommandHandler> _logger = logger;

    public async Task<ErrorOr<Success>> Handle(
        ProcessStripeWebhookCommand request,
        CancellationToken cancellationToken
    )
    {
        var parseResult = _paymentService.ParseWebhookEvent(
            request.JsonPayload,
            request.SignatureHeader
        );

        if (parseResult.IsError)
        {
            return parseResult.Errors;
        }

        var webhookEvent = parseResult.Value;

        if (webhookEvent.EventType == PaymentWebhookEventType.Unknown)
        {
            return Result.Success;
        }

        if (
            string.IsNullOrWhiteSpace(webhookEvent.OrderId)
            || !Guid.TryParse(webhookEvent.OrderId, out var orderGuid)
        )
        {
            return Result.Success;
        }

        var orderId = OrderId.Create(orderGuid);
        var order = await _orderRepository.GetOrderByIdAsync(orderId);
        var payment = await _paymentRepository.GetPaymentByOrderIdAsync(orderId);

        switch (webhookEvent.EventType)
        {
            case PaymentWebhookEventType.PaymentSucceeded:
                await HandlePaymentSucceededAsync(order, payment, webhookEvent, cancellationToken);
                break;

            case PaymentWebhookEventType.PaymentFailed:
                await HandlePaymentFailedAsync(payment, webhookEvent, cancellationToken);
                break;

            case PaymentWebhookEventType.ChargeRefunded:
                await HandleChargeRefundedAsync(order, payment, cancellationToken);
                break;
        }

        return Result.Success;
    }

    private async Task HandlePaymentSucceededAsync(
        Domain.Orders.Order? order,
        Domain.Payments.Payment? payment,
        PaymentWebhookEvent webhookEvent,
        CancellationToken cancellationToken
    )
    {
        var chargeOrIntentId =
            webhookEvent.ChargeId ?? webhookEvent.PaymentIntentId ?? string.Empty;
        var customerId = webhookEvent.CustomerId ?? string.Empty;

        if (payment is not null && payment.PaymentStatus != PaymentStatus.Payed)
        {
            payment.Complete(chargeOrIntentId, customerId);
            _paymentRepository.Update(payment);
        }

        if (order is not null && order.OrderStatus == OrderStatus.Pending)
        {
            order.CompletePayment(customerId);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandlePaymentFailedAsync(
        Domain.Payments.Payment? payment,
        PaymentWebhookEvent webhookEvent,
        CancellationToken cancellationToken
    )
    {
        if (payment is not null && payment.PaymentStatus == PaymentStatus.Pending)
        {
            payment.UpdatePaymentStatus(PaymentStatus.Cancelled);
            _paymentRepository.Update(payment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task HandleChargeRefundedAsync(
        Domain.Orders.Order? order,
        Domain.Payments.Payment? payment,
        CancellationToken cancellationToken
    )
    {
        if (payment is not null && payment.PaymentStatus != PaymentStatus.Refunded)
        {
            payment.UpdatePaymentStatus(PaymentStatus.Refunded);
            _paymentRepository.Update(payment);
        }

        if (
            order is not null
            && order.OrderStatus != OrderStatus.Cancelled
            && order.OrderStatus != OrderStatus.Refunded
        )
        {
            order.CancelOrder("Refunded via payment gateway");
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Application.Common.Interfaces.Services;
using Shopizy.Domain.Orders.Events;
using Shopizy.Domain.Payments.Enums;
using Shopizy.SharedKernel.Application.Interfaces.Persistence;
using Shopizy.SharedKernel.Application.Messaging;

namespace Shopizy.Application.Orders.Events;

public class OrderCancelledDomainEventHandler(
    IProductRepository productRepository,
    IPaymentRepository paymentRepository,
    IPaymentService paymentService,
    IUnitOfWork unitOfWork
) : IDomainEventHandler<OrderCancelledDomainEvent>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IPaymentRepository _paymentRepository = paymentRepository;
    private readonly IPaymentService _paymentService = paymentService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(
        OrderCancelledDomainEvent domainEvent,
        CancellationToken cancellationToken = default
    )
    {
        var hasChanges = false;

        // 1. Restore product inventory
        var productIds = domainEvent.Order.OrderItems.Select(i => i.ProductId).ToList();
        if (productIds.Count > 0)
        {
            var products = await _productRepository.GetProductsByIdsForUpdateAsync(productIds);
            foreach (var item in domainEvent.Order.OrderItems)
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product is not null)
                {
                    product.IncreaseStock(item.Quantity);
                    _productRepository.Update(product);
                    hasChanges = true;
                }
            }
        }

        // 2. Refund payment if already completed
        var payment = await _paymentRepository.GetPaymentByOrderIdAsync(domainEvent.Order.Id);
        if (
            payment is not null
            && payment.PaymentStatus == PaymentStatus.Payed
            && !string.IsNullOrEmpty(payment.TransactionId)
        )
        {
            var refundResult = await _paymentService.CreateRefundAsync(
                payment.TransactionId,
                cancellationToken
            );

            if (!refundResult.IsError)
            {
                payment.UpdatePaymentStatus(PaymentStatus.Refunded);
                _paymentRepository.Update(payment);
                hasChanges = true;
            }
        }

        if (hasChanges)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

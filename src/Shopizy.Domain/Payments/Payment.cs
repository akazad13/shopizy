using shopizy.Domain.Payments.Enums;
using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Customers.ValueObject;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Payments.ValueObjects;

namespace Shopizy.Domain.Payments;

public sealed class Payment : Entity<PaymentId>
{
    public OrderId OrderId { get; set; }
    public CustomerId CustomerId { get; set; }
    public string PaymentMethod { get; private set; }
    public string TransactionId { get; private set; }
    public PaymentStatus PaymentStatus { get; private set; }
    public Price Total { get; private set; }
    public Address BillingAddress { get; }
    public DateTime CreatedOn { get; private set; }
    public DateTime ModifiedOn { get; private set; }

    public static Payment Create(
        CustomerId customerId,
        OrderId orderId,
        string paymentMethod,
        string transactionId,
        PaymentStatus paymentStatus,
        Price total,
        Address billingAddress
    )
    {
        return new Payment(
            PaymentId.CreateUnique(),
            customerId,
            orderId,
            paymentMethod,
            transactionId,
            paymentStatus,
            total,
            billingAddress,
            DateTime.UtcNow,
            DateTime.UtcNow
        );
    }

    private Payment(
        PaymentId PaymentId,
        CustomerId customerId,
        OrderId orderId,
        string paymentMethod,
        string transactionId,
        PaymentStatus paymentStatus,
        Price total,
        Address billingAddress,
        DateTime createdOn,
        DateTime modifiedOn
    ) : base(PaymentId)
    {
        CustomerId = customerId;
        OrderId = orderId;
        PaymentMethod = paymentMethod;
        TransactionId = transactionId;
        PaymentStatus = paymentStatus;
        Total = total;
        BillingAddress = billingAddress;
        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
    }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Payment() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

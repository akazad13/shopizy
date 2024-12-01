using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Common.ValueObjects;
using Shopizy.Domain.Orders;
using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Payments.Enums;
using Shopizy.Domain.Payments.ValueObjects;
using Shopizy.Domain.Users;
using Shopizy.Domain.Users.ValueObjects;

namespace Shopizy.Domain.Payments;

public sealed class Payment : Entity<PaymentId>
{
    public Order Order { get; set; } = null!;
    public OrderId OrderId { get; set; }
    public User User { get; set; } = null!;
    public UserId UserId { get; set; }
    public string PaymentMethod { get; private set; }
    public string TransactionId { get; private set; }
    public PaymentStatus PaymentStatus { get; private set; }
    public Price Total { get; private set; }
    public Address BillingAddress { get; }
    public DateTime CreatedOn { get; private set; }
    public DateTime? ModifiedOn { get; private set; }

    public static Payment Create(
        UserId userId,
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
            userId,
            orderId,
            paymentMethod,
            transactionId,
            paymentStatus,
            total,
            billingAddress
        );
    }

    private Payment(
        PaymentId PaymentId,
        UserId userId,
        OrderId orderId,
        string paymentMethod,
        string transactionId,
        PaymentStatus paymentStatus,
        Price total,
        Address billingAddress
    )
        : base(PaymentId)
    {
        UserId = userId;
        OrderId = orderId;
        PaymentMethod = paymentMethod;
        TransactionId = transactionId;
        PaymentStatus = paymentStatus;
        Total = total;
        BillingAddress = billingAddress;
        CreatedOn = DateTime.UtcNow;
    }

    private Payment() { }

    public void UpdatePaymentStatus(PaymentStatus status)
    {
        PaymentStatus = status;
        ModifiedOn = DateTime.UtcNow;
    }
}

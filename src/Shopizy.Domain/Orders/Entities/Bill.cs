using Shopizy.Domain.Common.Models;
using Shopizy.Domain.Orders.ValueObjects;

namespace Shopizy.Domain.Orders.Entities;

public sealed class Bill : Entity<BillId>
{
    public string PaymentMethod { get; private set; }
    public string TransactionId { get; private set; }
    public string BillingStatus { get; private set; }
    public decimal Total { get; private set; }
    public Address BillingAddress { get; }
    public DateTime CreatedOn { get; private set; }
    public DateTime ModifiedOn { get; private set; }

    public static Bill Create(
        string paymentMethod,
        string transactionId,
        string billingStatus,
        decimal total,
        Address billingAddress
    )
    {
        return new Bill(
            BillId.CreateUnique(),
            paymentMethod,
            transactionId,
            billingStatus,
            total,
            billingAddress,
            DateTime.UtcNow,
            DateTime.UtcNow
        );
    }

    private Bill(
        BillId billId,
        string paymentMethod,
        string transactionId,
        string billingStatus,
        decimal total,
        Address billingAddress,
        DateTime createdOn,
        DateTime modifiedOn
    ) : base(billId)
    {
        PaymentMethod = paymentMethod;
        TransactionId = transactionId;
        BillingStatus = billingStatus;
        Total = total;
        BillingAddress = billingAddress;
        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
    }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Bill() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

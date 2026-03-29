using Shopizy.Domain.Orders.ValueObjects;
using Shopizy.Domain.Payments;
using Shopizy.Domain.Payments.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Persistence;

public interface IPaymentRepository
{
    Task<IReadOnlyList<Payment>> GetPaymentsAsync();
    Task<Payment?> GetPaymentByIdAsync(PaymentId id);
    Task<Payment?> GetPaymentByOrderIdAsync(OrderId orderId);
    Task AddAsync(Payment payment);
    void Update(Payment payment);
}


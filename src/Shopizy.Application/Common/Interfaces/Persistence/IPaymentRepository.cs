using Shopizy.Domain.Payments;
using Shopizy.Domain.Payments.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Persistence;

public interface IPaymentRepository
{
    Task<List<Payment>> GetPaymentsAsync();
    Task<Payment?> GetPaymentByIdAsync(PaymentId id);
    Task AddAsync(Payment payment);
    void Update(Payment payment);
    Task<int> CommitAsync(CancellationToken cancellationToken);
}

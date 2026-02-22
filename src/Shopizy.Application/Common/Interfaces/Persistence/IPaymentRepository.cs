using System.Collections.Generic;
using System.Threading.Tasks;
using Shopizy.Domain.Payments;
using Shopizy.Domain.Payments.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Persistence;

public interface IPaymentRepository
{
    Task<IReadOnlyList<Payment>> GetPaymentsAsync();
    Task<Payment?> GetPaymentByIdAsync(PaymentId id);
    Task AddAsync(Payment payment);
    void Update(Payment payment);
}

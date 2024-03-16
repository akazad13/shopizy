using Microsoft.EntityFrameworkCore;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Domain.Payments;
using Shopizy.Domain.Payments.ValueObjects;
using Shopizy.Infrastructure.Common.Persistence;

namespace Shopizy.Infrastructure.Customers.Persistence;

public class PaymentRepository(AppDbContext _dbContext) : IPaymentRepository
{
    public Task<List<Payment>> GetPaymentsAsync()
    {
        return _dbContext.Payments.AsNoTracking().ToListAsync();
    }
    public Task<Payment?> GetPaymentByIdAsync(PaymentId id)
    {
        return _dbContext.Payments.FirstOrDefaultAsync(c => c.Id == id);
    }
    public async Task AddAsync(Payment payment)
    {
        await _dbContext.Payments.AddAsync(payment);
    }
    public void Update(Payment payment)
    {
        _dbContext.Update(payment);
    }

    public Task<int> Commit(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shopizy.Application.Common.Interfaces.Persistence;
using Shopizy.Domain.Payments;
using Shopizy.Domain.Payments.ValueObjects;
using Shopizy.Infrastructure.Common.Persistence;

namespace Shopizy.Infrastructure.Payments.Persistence;

/// <summary>
/// Repository for managing payment data persistence.
/// </summary>
public class PaymentRepository(AppDbContext dbContext) : IPaymentRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    /// <summary>
    /// Retrieves all payments from the database.
    /// </summary>
    /// <returns>A list of all payments.</returns>
    public async Task<IReadOnlyList<Payment>> GetPaymentsAsync()
    {
        return await _dbContext.Payments.AsNoTracking().ToListAsync();
    }

    /// <summary>
    /// Retrieves a payment by its unique identifier.
    /// </summary>
    /// <param name="id">The payment identifier.</param>
    /// <returns>The payment if found; otherwise, null.</returns>
    public Task<Payment?> GetPaymentByIdAsync(PaymentId id)
    {
        return _dbContext.Payments.FirstOrDefaultAsync(c => c.Id == id);
    }

    /// <summary>
    /// Adds a new payment to the database.
    /// </summary>
    /// <param name="payment">The payment to add.</param>
    public async Task AddAsync(Payment payment)
    {
        await _dbContext.Payments.AddAsync(payment);
    }

    /// <summary>
    /// Updates an existing payment in the database.
    /// </summary>
    /// <param name="payment">The payment to update.</param>
    public void Update(Payment payment)
    {
        _dbContext.Update(payment);
    }
}

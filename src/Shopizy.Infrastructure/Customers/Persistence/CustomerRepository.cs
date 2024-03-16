using Microsoft.EntityFrameworkCore;
using Shopizy.Application.Common.Interfaces.Persistance;
using Shopizy.Domain.Customers;
using Shopizy.Domain.Customers.ValueObjects;
using Shopizy.Infrastructure.Common.Persistence;

namespace Shopizy.Infrastructure.Customers.Persistence;

public class CustomerRepository(AppDbContext _dbContext) : ICustomerRepository
{
    public Task<List<Customer>> GetCustomersAsync()
    {
        return _dbContext.Customers.AsNoTracking().ToListAsync();
    }
    public Task<Customer?> GetCustomerByIdAsync(CustomerId id)
    {
        return _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == id);
    }
    public async Task AddAsync(Customer customer)
    {
        await _dbContext.Customers.AddAsync(customer);
    }
    public void Update(Customer customer)
    {
        _dbContext.Update(customer);
    }

    public Task<int> Commit(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
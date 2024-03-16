using Shopizy.Domain.Customers;
using Shopizy.Domain.Customers.ValueObjects;

namespace Shopizy.Application.Common.Interfaces.Persistance;

public interface ICustomerRepository
{
    Task<Customer?> GetCustomerByIdAsync(CustomerId id);
    Task<List<Customer>> GetCustomersAsync();
    Task AddAsync(Customer customer);
    void Update(Customer customer);
    Task<int> Commit(CancellationToken cancellationToken);
}

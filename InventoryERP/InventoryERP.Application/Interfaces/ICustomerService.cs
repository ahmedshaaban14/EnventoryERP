using InventoryERP.Domain.Entities;

namespace InventoryERP.Application.Interfaces;

public interface ICustomerService
{
    Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Customer?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Customer> AddAsync(Customer customer, CancellationToken cancellationToken = default);
    Task<Customer> UpdateAsync(Customer customer, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

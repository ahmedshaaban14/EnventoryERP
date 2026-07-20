using InventoryERP.Domain.Entities;

namespace InventoryERP.Application.Interfaces;

public interface ISupplierService
{
    Task<IReadOnlyList<Supplier>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Supplier?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Supplier?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Supplier> AddAsync(Supplier supplier, CancellationToken cancellationToken = default);
    Task<Supplier> UpdateAsync(Supplier supplier, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

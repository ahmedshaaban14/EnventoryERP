using InventoryERP.Domain.Entities;

namespace InventoryERP.Application.Interfaces;

public interface IItemService
{
    Task<IReadOnlyList<Item>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Item?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Item?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Item> AddAsync(Item item, CancellationToken cancellationToken = default);
    Task<Item> UpdateAsync(Item item, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

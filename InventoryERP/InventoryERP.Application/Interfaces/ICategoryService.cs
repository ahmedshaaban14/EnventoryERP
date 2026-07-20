using InventoryERP.Domain.Entities;

namespace InventoryERP.Application.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default);
}

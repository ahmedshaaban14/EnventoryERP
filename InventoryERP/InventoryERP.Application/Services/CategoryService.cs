using InventoryERP.Application.Interfaces;
using InventoryERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryERP.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IInventoryDbContext _context;

    public CategoryService(IInventoryDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .AsNoTracking()
            .OrderBy(category => category.Name)
            .ToListAsync(cancellationToken);
    }
}

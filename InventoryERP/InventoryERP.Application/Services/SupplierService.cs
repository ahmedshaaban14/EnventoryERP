using InventoryERP.Application.Interfaces;
using InventoryERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryERP.Application.Services;

public class SupplierService : ISupplierService
{
    private readonly IInventoryDbContext _context;

    public SupplierService(IInventoryDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Supplier>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Suppliers
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Supplier?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Suppliers
            .Where(x => x.IsActive && x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Supplier?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Suppliers
            .Where(x => x.IsActive && x.Code == code)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Supplier> AddAsync(Supplier supplier, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(supplier.Code))
        {
            supplier.Code = await GenerateCodeAsync(cancellationToken);
        }

        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync(cancellationToken);
        return supplier;
    }

    public async Task<Supplier> UpdateAsync(Supplier supplier, CancellationToken cancellationToken = default)
    {
        var existing = await GetByIdAsync(supplier.Id, cancellationToken);
        if (existing == null)
            throw new InvalidOperationException("Supplier not found");

        existing.Name = supplier.Name;
        existing.Phone = supplier.Phone;
        existing.Address = supplier.Address;
        existing.Email = supplier.Email;
        existing.TaxNumber = supplier.TaxNumber;
        existing.Notes = supplier.Notes;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var supplier = await GetByIdAsync(id, cancellationToken);
        if (supplier != null)
        {
            supplier.IsActive = false;
            supplier.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task<string> GenerateCodeAsync(CancellationToken cancellationToken)
    {
        var last = await _context.Suppliers
            .Where(x => x.Code.StartsWith("SUP-"))
            .OrderByDescending(x => x.Code)
            .Select(x => x.Code)
            .FirstOrDefaultAsync(cancellationToken);

        var number = 1;
        if (!string.IsNullOrWhiteSpace(last))
        {
            var numberStr = last.Replace("SUP-", string.Empty);
            if (int.TryParse(numberStr, out var n))
            {
                number = n + 1;
            }
        }

        return $"SUP-{number:D6}";
    }
}

using InventoryERP.Application.Interfaces;
using InventoryERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryERP.Application.Services;

public class ItemService : IItemService
{
    private readonly IInventoryDbContext _context;

    public ItemService(IInventoryDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Item>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Items
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Item?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Items
            .Where(x => x.IsActive && x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Item?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Items
            .Where(x => x.IsActive && x.Code == code)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Item> AddAsync(Item item, CancellationToken cancellationToken = default)
    {
        NormalizeItem(item);
        item.Code = await GenerateCodeAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(item.Barcode))
        {
            item.Barcode = item.Code;
        }

        await EnsureUniqueAsync(item, cancellationToken);

        _context.Items.Add(item);
        await _context.SaveChangesAsync(cancellationToken);
        return item;
    }

    public async Task<Item> UpdateAsync(Item item, CancellationToken cancellationToken = default)
    {
        var existing = await GetByIdAsync(item.Id, cancellationToken);
        if (existing == null)
            throw new InvalidOperationException("Item not found");

        NormalizeItem(item);
        await EnsureUniqueAsync(item, cancellationToken, existing.Id);

        existing.Name = item.Name;
        existing.EnglishName = item.EnglishName;
        existing.CategoryId = item.CategoryId;
        existing.Brand = item.Brand;
        existing.Car = item.Car;
        existing.Model = item.Model;
        existing.Year = item.Year;
        existing.Barcode = string.IsNullOrWhiteSpace(item.Barcode) ? existing.Barcode : item.Barcode.Trim();
        existing.Unit = item.Unit;
        existing.MinimumStock = item.MinimumStock;
        existing.Rack = item.Rack;
        existing.Shelf = item.Shelf;
        existing.SalePrice = item.SalePrice;
        existing.WholesalePrice = item.WholesalePrice;
        existing.ImagePath = item.ImagePath;
        existing.Notes = item.Notes;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await GetByIdAsync(id, cancellationToken);
        if (item != null)
        {
            item.IsActive = false;
            item.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task<string> GenerateCodeAsync(CancellationToken cancellationToken)
    {
        var last = await _context.Items
            .Where(x => x.Code.StartsWith("ITM-"))
            .OrderByDescending(x => x.Id)
            .Select(x => x.Code)
            .FirstOrDefaultAsync(cancellationToken);

        var number = 1;
        if (!string.IsNullOrWhiteSpace(last))
        {
            var numberStr = last.Replace("ITM-", string.Empty);
            if (int.TryParse(numberStr, out var n))
            {
                number = n + 1;
            }
        }

        return $"ITM-{number:D6}";
    }

    private static void NormalizeItem(Item item)
    {
        item.Code = item.Code.Trim();
        item.Barcode = item.Barcode.Trim();
        item.Name = item.Name.Trim();
        item.EnglishName = item.EnglishName?.Trim();
        item.Brand = item.Brand.Trim();
        item.Car = item.Car.Trim();
        item.Model = item.Model.Trim();
        item.Unit = string.IsNullOrWhiteSpace(item.Unit) ? "قطعة" : item.Unit.Trim();
        item.Rack = item.Rack.Trim();
        item.Shelf = item.Shelf.Trim();
        item.ImagePath = item.ImagePath?.Trim();
        item.Notes = item.Notes?.Trim();
    }

    private async Task EnsureUniqueAsync(Item item, CancellationToken cancellationToken, int? ignoredId = null)
    {
        var query = _context.Items.AsQueryable();
        if (ignoredId.HasValue)
        {
            query = query.Where(x => x.Id != ignoredId.Value);
        }

        if (await query.AnyAsync(x => x.Code == item.Code, cancellationToken))
        {
            throw new InvalidOperationException("Item code already exists.");
        }

        if (!string.IsNullOrWhiteSpace(item.Barcode) &&
            await query.AnyAsync(x => x.Barcode == item.Barcode, cancellationToken))
        {
            throw new InvalidOperationException("Barcode already exists.");
        }
    }
}

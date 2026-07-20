using CommunityToolkit.Mvvm.ComponentModel;
using InventoryERP.Application.Interfaces;
using InventoryERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace InventoryERP.App;

public partial class InventoryViewModel : ObservableObject
{
    private readonly IInventoryDbContext _context;

    [ObservableProperty]
    private ObservableCollection<StockMovement> stockMovements = new();

    [ObservableProperty]
    private ObservableCollection<Item> lowStockItems = new();

    [ObservableProperty]
    private string statusMessage = string.Empty;

    public InventoryViewModel(IInventoryDbContext context)
    {
        _context = context;
    }

    public async Task LoadAsync()
    {
        var movements = await _context.StockMovements
            .AsNoTracking()
            .Include(movement => movement.Item)
            .OrderByDescending(movement => movement.MovementDate)
            .Take(50)
            .ToListAsync();

        var items = await _context.Items
            .AsNoTracking()
            .Where(item => item.IsActive && item.CurrentStock <= item.MinimumStock)
            .OrderBy(item => item.Name)
            .ToListAsync();

        StockMovements = new ObservableCollection<StockMovement>(movements);
        LowStockItems = new ObservableCollection<Item>(items);
        StatusMessage = $"تم تحميل {movements.Count} حركة مخزون";
    }
}

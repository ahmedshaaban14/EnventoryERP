using CommunityToolkit.Mvvm.ComponentModel;
using InventoryERP.Application.Common;
using InventoryERP.Application.Interfaces;
using InventoryERP.Domain.Entities;
using System.Collections.ObjectModel;

namespace InventoryERP.App;

public partial class ReportsViewModel : ObservableObject
{
    private readonly IReportService _reportService;

    [ObservableProperty]
    private InventoryReport? inventoryReport;

    [ObservableProperty]
    private ObservableCollection<Item> lowStockItems = new();

    [ObservableProperty]
    private ObservableCollection<StockMovement> recentMovements = new();

    [ObservableProperty]
    private string statusMessage = string.Empty;

    public ReportsViewModel(IReportService reportService)
    {
        _reportService = reportService;
    }

    public async Task LoadAsync()
    {
        var report = await _reportService.GetInventoryReportAsync();
        InventoryReport = report;
        LowStockItems = new ObservableCollection<Item>(report.LowStockItems);
        RecentMovements = new ObservableCollection<StockMovement>(report.RecentMovements);
        StatusMessage = $"تم تحديث التقرير في {DateTime.Now:t}";
    }
}

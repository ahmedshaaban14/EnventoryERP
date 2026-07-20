using CommunityToolkit.Mvvm.ComponentModel;
using InventoryERP.Application.Interfaces;
using InventoryERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace InventoryERP.App;

public partial class CashViewModel : ObservableObject
{
    private readonly IInventoryDbContext _context;

    [ObservableProperty]
    private ObservableCollection<CashSafe> cashSafes = new();

    [ObservableProperty]
    private ObservableCollection<CashMovement> cashMovements = new();

    [ObservableProperty]
    private CashSafe? selectedCashSafe;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    [ObservableProperty]
    private decimal currentBalance;

    [ObservableProperty]
    private decimal totalReceipts;

    [ObservableProperty]
    private decimal totalPayments;

    public CashViewModel(IInventoryDbContext context)
    {
        _context = context;
    }

    public async Task LoadAsync()
    {
        var safes = await _context.CashSafes.AsNoTracking().ToListAsync();
        CashSafes = new ObservableCollection<CashSafe>(safes);
        SelectedCashSafe = CashSafes.FirstOrDefault();
        StatusMessage = CashSafes.Count == 0 ? "لا توجد خزنة معرفة بعد." : $"تم تحميل {CashSafes.Count} خزنة";
    }

    partial void OnSelectedCashSafeChanged(CashSafe? value)
    {
        if (value == null)
        {
            CashMovements = new ObservableCollection<CashMovement>();
            CurrentBalance = 0;
            TotalReceipts = 0;
            TotalPayments = 0;
            return;
        }

        _ = LoadMovementsAsync(value.Id);
        CurrentBalance = value.CurrentBalance;
    }

    private async Task LoadMovementsAsync(int cashSafeId)
    {
        var movements = await _context.CashMovements
            .AsNoTracking()
            .Where(movement => movement.CashSafeId == cashSafeId)
            .OrderByDescending(movement => movement.MovementDate)
            .ToListAsync();

        CashMovements = new ObservableCollection<CashMovement>(movements);
        TotalReceipts = movements.Where(movement => movement.OperationType == "قبض").Sum(movement => movement.Amount);
        TotalPayments = movements.Where(movement => movement.OperationType == "صرف").Sum(movement => movement.Amount);
    }
}

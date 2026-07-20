using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventoryERP.Application.Interfaces;
using InventoryERP.Domain.Entities;
using System.Collections.ObjectModel;

namespace InventoryERP.App;

public partial class SuppliersViewModel : ObservableObject
{
    private readonly ISupplierService _supplierService;
    private readonly List<Supplier> _allSuppliers = new();

    [ObservableProperty]
    private ObservableCollection<Supplier> suppliers = new();

    [ObservableProperty]
    private Supplier? selectedSupplier;

    [ObservableProperty]
    private Supplier editorSupplier = CreateEditorSupplier();

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    public SuppliersViewModel(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    public async Task LoadAsync()
    {
        var suppliers = await _supplierService.GetAllAsync();
        _allSuppliers.Clear();
        _allSuppliers.AddRange(suppliers);
        ApplyFilter();
        StatusMessage = $"تم تحميل {Suppliers.Count} مورد";
    }

    [RelayCommand]
    private void NewSupplier()
    {
        SelectedSupplier = null;
        EditorSupplier = CreateEditorSupplier();
        StatusMessage = "جاهز لإضافة مورد جديد";
    }

    [RelayCommand]
    private async Task SaveSupplierAsync()
    {
        ValidateEditor();
        if (EditorSupplier.Id == 0)
        {
            await _supplierService.AddAsync(EditorSupplier);
            StatusMessage = "تمت إضافة المورد";
        }
        else
        {
            await _supplierService.UpdateAsync(EditorSupplier);
            StatusMessage = "تم تحديث المورد";
        }

        await LoadAsync();
    }

    [RelayCommand]
    private async Task DeleteSupplierAsync()
    {
        if (SelectedSupplier == null)
        {
            return;
        }

        await _supplierService.DeleteAsync(SelectedSupplier.Id);
        SelectedSupplier = null;
        EditorSupplier = CreateEditorSupplier();
        StatusMessage = "تم حذف المورد";
        await LoadAsync();
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadAsync();
    }

    partial void OnSelectedSupplierChanged(Supplier? value)
    {
        EditorSupplier = value == null ? CreateEditorSupplier() : Clone(value);
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var query = (SearchText ?? string.Empty).Trim();
        var comparison = StringComparison.OrdinalIgnoreCase;
        var filtered = string.IsNullOrWhiteSpace(query)
            ? _allSuppliers.ToList()
            : _allSuppliers.Where(supplier =>
                (supplier.Code ?? string.Empty).Contains(query, comparison) ||
                (supplier.Name ?? string.Empty).Contains(query, comparison) ||
                (supplier.Phone ?? string.Empty).Contains(query, comparison) ||
                (supplier.Address ?? string.Empty).Contains(query, comparison))
            .ToList();

        Suppliers = new ObservableCollection<Supplier>(filtered);
    }

    private static Supplier CreateEditorSupplier()
    {
        return new Supplier { IsActive = true };
    }

    private static Supplier Clone(Supplier supplier)
    {
        return new Supplier
        {
            Id = supplier.Id,
            Code = supplier.Code,
            Name = supplier.Name,
            Phone = supplier.Phone,
            Address = supplier.Address,
            Email = supplier.Email,
            TaxNumber = supplier.TaxNumber,
            Notes = supplier.Notes,
            OpeningBalance = supplier.OpeningBalance,
            TotalPurchases = supplier.TotalPurchases,
            TotalPayments = supplier.TotalPayments,
            IsActive = supplier.IsActive
        };
    }

    private void ValidateEditor()
    {
        if (string.IsNullOrWhiteSpace(EditorSupplier.Name))
        {
            throw new InvalidOperationException("اسم المورد مطلوب.");
        }
    }
}

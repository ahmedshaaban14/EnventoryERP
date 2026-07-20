using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventoryERP.Application.Interfaces;
using InventoryERP.Domain.Entities;
using System.Collections.ObjectModel;

namespace InventoryERP.App;

public partial class CustomersViewModel : ObservableObject
{
    private readonly ICustomerService _customerService;
    private readonly List<Customer> _allCustomers = new();

    [ObservableProperty]
    private ObservableCollection<Customer> customers = new();

    [ObservableProperty]
    private Customer? selectedCustomer;

    [ObservableProperty]
    private Customer editorCustomer = CreateEditorCustomer();

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    public CustomersViewModel(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public async Task LoadAsync()
    {
        var customers = await _customerService.GetAllAsync();
        _allCustomers.Clear();
        _allCustomers.AddRange(customers);
        ApplyFilter();
        StatusMessage = $"تم تحميل {Customers.Count} عميل";
    }

    [RelayCommand]
    private void NewCustomer()
    {
        SelectedCustomer = null;
        EditorCustomer = CreateEditorCustomer();
        StatusMessage = "جاهز لإضافة عميل جديد";
    }

    [RelayCommand]
    private async Task SaveCustomerAsync()
    {
        ValidateEditor();
        if (EditorCustomer.Id == 0)
        {
            await _customerService.AddAsync(EditorCustomer);
            StatusMessage = "تمت إضافة العميل";
        }
        else
        {
            await _customerService.UpdateAsync(EditorCustomer);
            StatusMessage = "تم تحديث العميل";
        }

        await LoadAsync();
    }

    [RelayCommand]
    private async Task DeleteCustomerAsync()
    {
        if (SelectedCustomer == null)
        {
            return;
        }

        await _customerService.DeleteAsync(SelectedCustomer.Id);
        SelectedCustomer = null;
        EditorCustomer = CreateEditorCustomer();
        StatusMessage = "تم حذف العميل";
        await LoadAsync();
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadAsync();
    }

    partial void OnSelectedCustomerChanged(Customer? value)
    {
        EditorCustomer = value == null ? CreateEditorCustomer() : Clone(value);
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
            ? _allCustomers.ToList()
            : _allCustomers.Where(customer =>
                (customer.Code ?? string.Empty).Contains(query, comparison) ||
                (customer.Name ?? string.Empty).Contains(query, comparison) ||
                (customer.Phone ?? string.Empty).Contains(query, comparison) ||
                (customer.Address ?? string.Empty).Contains(query, comparison))
            .ToList();

        Customers = new ObservableCollection<Customer>(filtered);
    }

    private static Customer CreateEditorCustomer()
    {
        return new Customer { IsActive = true };
    }

    private static Customer Clone(Customer customer)
    {
        return new Customer
        {
            Id = customer.Id,
            Code = customer.Code,
            Name = customer.Name,
            Phone = customer.Phone,
            Address = customer.Address,
            Car = customer.Car,
            Notes = customer.Notes,
            OpeningBalance = customer.OpeningBalance,
            TotalSales = customer.TotalSales,
            TotalPayments = customer.TotalPayments,
            CreditLimit = customer.CreditLimit,
            LastPurchaseDate = customer.LastPurchaseDate,
            TotalPurchases = customer.TotalPurchases,
            IsActive = customer.IsActive
        };
    }

    private void ValidateEditor()
    {
        if (string.IsNullOrWhiteSpace(EditorCustomer.Name))
        {
            throw new InvalidOperationException("اسم العميل مطلوب.");
        }
    }
}

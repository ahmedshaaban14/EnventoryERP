using System.ComponentModel;
using System.Runtime.CompilerServices;
using InventoryERP.Application.Interfaces;
using InventoryERP.Domain.Entities;

namespace InventoryERP.App;

public class InvoicesViewModel : INotifyPropertyChanged
{
    private readonly IInvoiceService _invoiceService;
    private readonly IItemService _itemService;
    private readonly ICustomerService _customerService;
    private readonly ISupplierService _supplierService;
    private List<SaleInvoice> _saleInvoices = new();
    private List<PurchaseInvoice> _purchaseInvoices = new();
    private SaleInvoice? _selectedSaleInvoice;
    private PurchaseInvoice? _selectedPurchaseInvoice;
    private List<Item> _items = new();
    private List<Customer> _customers = new();
    private List<Supplier> _suppliers = new();
    private bool _isSaleTab = true;

    public InvoicesViewModel(IInvoiceService invoiceService, IItemService itemService, ICustomerService customerService, ISupplierService supplierService)
    {
        _invoiceService = invoiceService;
        _itemService = itemService;
        _customerService = customerService;
        _supplierService = supplierService;
    }

    public List<SaleInvoice> SaleInvoices
    {
        get => _saleInvoices;
        set
        {
            _saleInvoices = value;
            OnPropertyChanged();
        }
    }

    public List<PurchaseInvoice> PurchaseInvoices
    {
        get => _purchaseInvoices;
        set
        {
            _purchaseInvoices = value;
            OnPropertyChanged();
        }
    }

    public SaleInvoice? SelectedSaleInvoice
    {
        get => _selectedSaleInvoice;
        set
        {
            _selectedSaleInvoice = value;
            OnPropertyChanged();
        }
    }

    public PurchaseInvoice? SelectedPurchaseInvoice
    {
        get => _selectedPurchaseInvoice;
        set
        {
            _selectedPurchaseInvoice = value;
            OnPropertyChanged();
        }
    }

    public List<Item> Items
    {
        get => _items;
        set
        {
            _items = value;
            OnPropertyChanged();
        }
    }

    public List<Customer> Customers
    {
        get => _customers;
        set
        {
            _customers = value;
            OnPropertyChanged();
        }
    }

    public List<Supplier> Suppliers
    {
        get => _suppliers;
        set
        {
            _suppliers = value;
            OnPropertyChanged();
        }
    }

    public bool IsSaleTab
    {
        get => _isSaleTab;
        set
        {
            _isSaleTab = value;
            OnPropertyChanged();
        }
    }

    public async Task LoadAsync()
    {
        SaleInvoices = (await _invoiceService.GetSaleInvoicesAsync()).ToList();
        PurchaseInvoices = (await _invoiceService.GetPurchaseInvoicesAsync()).ToList();
        Items = (await _itemService.GetAllAsync()).ToList();
        Customers = (await _customerService.GetAllAsync()).ToList();
        Suppliers = (await _supplierService.GetAllAsync()).ToList();
    }

    public async Task CreateSaleInvoiceAsync(SaleInvoice invoice)
    {
        await _invoiceService.CreateSaleInvoiceAsync(invoice);
        await LoadAsync();
    }

    public async Task CreatePurchaseInvoiceAsync(PurchaseInvoice invoice)
    {
        await _invoiceService.CreatePurchaseInvoiceAsync(invoice);
        await LoadAsync();
    }

    public async Task CancelSaleInvoiceAsync(int id)
    {
        await _invoiceService.CancelSaleInvoiceAsync(id);
        await LoadAsync();
    }

    public async Task CancelPurchaseInvoiceAsync(int id)
    {
        await _invoiceService.CancelPurchaseInvoiceAsync(id);
        await LoadAsync();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

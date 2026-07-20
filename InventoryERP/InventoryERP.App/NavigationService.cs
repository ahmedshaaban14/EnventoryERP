using InventoryERP.App.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace InventoryERP.App;

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<AppPage, object> _cache = new();

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public object Navigate(AppPage page) => GetPage(page) ?? throw new InvalidOperationException($"No page mapped for {page}.");

    public object? GetPage(AppPage page)
    {
        if (_cache.TryGetValue(page, out var existing))
        {
            return existing;
        }

        object? created = page switch
        {
            AppPage.Items => CreateItemsView(),
            AppPage.Customers => CreateCustomersView(),
            AppPage.Suppliers => CreateSuppliersView(),
            AppPage.PurchaseInvoices => CreateInvoicesView(false),
            AppPage.SalesInvoices => CreateInvoicesView(true),
            AppPage.Inventory => new InventoryView { DataContext = _serviceProvider.GetRequiredService<InventoryViewModel>() },
            AppPage.Cash => CreateCashView(),
            AppPage.Reports => CreateReportsView(),
            AppPage.Settings => new SettingsView { DataContext = _serviceProvider.GetRequiredService<SettingsViewModel>() },
            _ => null
        };

        if (created != null)
        {
            _cache[page] = created;
        }

        return created;
    }

    private UserControl CreateItemsView()
    {
        var view = new ItemsView { DataContext = _serviceProvider.GetRequiredService<ItemsViewModel>() };
        return view;
    }

    private UserControl CreateCustomersView()
    {
        var view = new CustomersView { DataContext = _serviceProvider.GetRequiredService<CustomersViewModel>() };
        return view;
    }

    private UserControl CreateSuppliersView()
    {
        var view = new SuppliersView { DataContext = _serviceProvider.GetRequiredService<SuppliersViewModel>() };
        return view;
    }

    private UserControl CreateInvoicesView(bool isSaleTab)
    {
        var viewModel = _serviceProvider.GetRequiredService<InvoicesViewModel>();
        viewModel.IsSaleTab = isSaleTab;
        return new InvoicesView { DataContext = viewModel };
    }

    private UserControl CreateCashView()
    {
        return new CashView { DataContext = _serviceProvider.GetRequiredService<CashViewModel>() };
    }

    private UserControl CreateReportsView()
    {
        return new ReportsView { DataContext = _serviceProvider.GetRequiredService<ReportsViewModel>() };
    }
}

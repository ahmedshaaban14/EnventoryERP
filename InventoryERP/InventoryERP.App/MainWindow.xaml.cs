using System.Windows;
using System.Windows.Controls;
using InventoryERP.App.Views;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryERP.App;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;
    private readonly IServiceProvider _serviceProvider;
    private bool _isSidebarCollapsed;

    public MainWindow(MainViewModel viewModel, IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _serviceProvider = serviceProvider;
        DataContext = _viewModel;
        Loaded += async (_, _) => await _viewModel.LoadAsync();
    }

    private void ToggleSidebarButton_Click(object sender, RoutedEventArgs e)
    {
        _isSidebarCollapsed = !_isSidebarCollapsed;
        SidebarColumn.Width = _isSidebarCollapsed ? new GridLength(84) : new GridLength(260);
    }

    private void DashboardButton_Click(object sender, RoutedEventArgs e)
    {
        DashboardContent.Visibility = Visibility.Visible;
        MainContent.Visibility = Visibility.Collapsed;
    }

    private async void ItemsButton_Click(object sender, RoutedEventArgs e) => await ShowViewAsync(async () =>
    {
        var itemsViewModel = _serviceProvider.GetRequiredService<ItemsViewModel>();
        var itemsView = new ItemsView
        {
            DataContext = itemsViewModel
        };

        MainContent.Content = itemsView;
        await itemsViewModel.LoadAsync();
    });

    private async void InvoicesButton_Click(object sender, RoutedEventArgs e) => await ShowViewAsync(async () =>
    {
        var invoicesViewModel = _serviceProvider.GetRequiredService<InvoicesViewModel>();
        var invoicesView = new InvoicesView
        {
            DataContext = invoicesViewModel
        };

        MainContent.Content = invoicesView;
        await invoicesViewModel.LoadAsync();
    });

    private async void CustomersButton_Click(object sender, RoutedEventArgs e) => await ShowViewAsync(async () =>
    {
        var customersViewModel = _serviceProvider.GetRequiredService<CustomersViewModel>();
        var customersView = new CustomersView
        {
            DataContext = customersViewModel
        };

        MainContent.Content = customersView;
        await customersViewModel.LoadAsync();
    });

    private async void SuppliersButton_Click(object sender, RoutedEventArgs e) => await ShowViewAsync(async () =>
    {
        var suppliersViewModel = _serviceProvider.GetRequiredService<SuppliersViewModel>();
        var suppliersView = new SuppliersView
        {
            DataContext = suppliersViewModel
        };

        MainContent.Content = suppliersView;
        await suppliersViewModel.LoadAsync();
    });

    private async void CashButton_Click(object sender, RoutedEventArgs e) => await ShowViewAsync(async () =>
    {
        var cashViewModel = _serviceProvider.GetRequiredService<CashViewModel>();
        var cashView = new CashView
        {
            DataContext = cashViewModel
        };

        MainContent.Content = cashView;
        await cashViewModel.LoadAsync();
    });

    private async void ReportsButton_Click(object sender, RoutedEventArgs e) => await ShowViewAsync(async () =>
    {
        var reportsViewModel = _serviceProvider.GetRequiredService<ReportsViewModel>();
        var reportsView = new ReportsView
        {
            DataContext = reportsViewModel
        };

        MainContent.Content = reportsView;
        await reportsViewModel.LoadAsync();
    });

    public async Task RefreshDashboardAsync()
    {
        await _viewModel.LoadAsync();
    }

    private static async Task ShowViewAsync(Func<Task> loadViewAsync)
    {
        try
        {
            await loadViewAsync();
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                $"تعذر فتح الشاشة المطلوبة.{Environment.NewLine}{Environment.NewLine}{exception.Message}",
                "ERP Application Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}

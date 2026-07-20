using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventoryERP.Application.Common;
using InventoryERP.Application.Interfaces;
using System.Collections.ObjectModel;
using System.Windows;

namespace InventoryERP.App;

public partial class MainViewModel : ObservableObject
{
    private readonly IDashboardService _dashboardService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private DashboardOverview? dashboard;

    [ObservableProperty]
    private object? currentPage;

    [ObservableProperty]
    private string currentUser = "مدير النظام";

    [ObservableProperty]
    private string companyName = "Inventory ERP";

    [ObservableProperty]
    private DateTime currentDateTime = DateTime.Now;

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private ObservableCollection<NavigationItemViewModel> navigationItems = new();

    public MainViewModel(IDashboardService dashboardService, INavigationService navigationService)
    {
        _dashboardService = dashboardService;
        _navigationService = navigationService;

        NavigationItems = new ObservableCollection<NavigationItemViewModel>
        {
            new() { Title = "لوحة التحكم", Icon = "⌂", Page = AppPage.Dashboard, IsSelected = true },
            new() { Title = "الأصناف", Icon = "▣", Page = AppPage.Items },
            new() { Title = "العملاء", Icon = "◉", Page = AppPage.Customers },
            new() { Title = "الموردين", Icon = "◈", Page = AppPage.Suppliers },
            new() { Title = "فواتير الشراء", Icon = "↧", Page = AppPage.PurchaseInvoices },
            new() { Title = "فواتير البيع", Icon = "↥", Page = AppPage.SalesInvoices },
            new() { Title = "المخزون", Icon = "▤", Page = AppPage.Inventory },
            new() { Title = "الخزنة", Icon = "₪", Page = AppPage.Cash },
            new() { Title = "التقارير", Icon = "▧", Page = AppPage.Reports },
            new() { Title = "الإعدادات", Icon = "⚙", Page = AppPage.Settings }
        };

        CurrentPage = _navigationService.GetPage(AppPage.Dashboard);
    }

    public async Task LoadAsync()
    {
        Dashboard = await _dashboardService.GetDashboardAsync();
        CurrentDateTime = Dashboard.CurrentDateTime;
        CurrentUser = Dashboard.CurrentUser;
        CompanyName = Dashboard.CompanyName;
    }

    [RelayCommand]
    private void Navigate(AppPage page)
    {
        CurrentPage = _navigationService.Navigate(page);

        foreach (var item in NavigationItems)
        {
            item.IsSelected = item.Page == page;
        }
    }

    [RelayCommand]
    private async Task RefreshDashboardAsync()
    {
        await LoadAsync();
        CurrentPage = _navigationService.GetPage(AppPage.Dashboard);
    }

    [RelayCommand]
    private void Logout()
    {
        CurrentUser = "غير مسجل";
        MessageBox.Show("تم تسجيل الخروج.", "Inventory ERP", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}

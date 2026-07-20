using System.Windows;
using System.Windows.Controls;

namespace InventoryERP.App.Views;

public partial class ItemsView : UserControl
{
    public ItemsView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is not ItemsViewModel viewModel)
        {
            return;
        }

        try
        {
            await viewModel.LoadAsync();
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                $"تعذر تحميل شاشة الأصناف.{Environment.NewLine}{Environment.NewLine}{exception.Message}",
                "ERP Application Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}

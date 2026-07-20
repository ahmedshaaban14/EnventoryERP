using System.Windows;
using System.Windows.Controls;

namespace InventoryERP.App.Views;

public partial class CustomersView : UserControl
{
    public CustomersView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is not CustomersViewModel viewModel)
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
                $"Unable to load customers screen.{Environment.NewLine}{Environment.NewLine}{exception.Message}",
                "ERP Application Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}

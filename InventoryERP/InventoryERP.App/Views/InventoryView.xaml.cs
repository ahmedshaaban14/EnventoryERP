using System.Windows;
using System.Windows.Controls;

namespace InventoryERP.App.Views;

public partial class InventoryView : UserControl
{
    public InventoryView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is not InventoryViewModel viewModel)
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
                $"Unable to load inventory screen.{Environment.NewLine}{Environment.NewLine}{exception.Message}",
                "ERP Application Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}

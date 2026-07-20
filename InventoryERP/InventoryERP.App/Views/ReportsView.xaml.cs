using System.Windows;
using System.Windows.Controls;

namespace InventoryERP.App.Views;

public partial class ReportsView : UserControl
{
    public ReportsView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is not ReportsViewModel viewModel)
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
                $"Unable to load reports screen.{Environment.NewLine}{Environment.NewLine}{exception.Message}",
                "ERP Application Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}

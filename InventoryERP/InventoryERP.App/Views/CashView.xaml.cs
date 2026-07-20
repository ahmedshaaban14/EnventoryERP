using System.Windows.Controls;

namespace InventoryERP.App.Views;

public partial class CashView : UserControl
{
    public CashView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is not CashViewModel viewModel)
        {
            return;
        }

        try
        {
            await viewModel.LoadAsync();
        }
        catch (Exception exception)
        {
            System.Windows.MessageBox.Show(
                $"Unable to load cash screen.{System.Environment.NewLine}{System.Environment.NewLine}{exception.Message}",
                "ERP Application Error",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
        }
    }
}

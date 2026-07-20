using System.Windows;
using System.Windows.Controls;
using InventoryERP.Domain.Entities;

namespace InventoryERP.App.Views;

public partial class InvoicesView : UserControl
{
    private InvoicesViewModel? _viewModel;

    public InvoicesView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        _viewModel = DataContext as InvoicesViewModel;
        if (_viewModel == null)
        {
            return;
        }

        try
        {
            await _viewModel.LoadAsync();
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                $"Unable to load invoices screen.{Environment.NewLine}{Environment.NewLine}{exception.Message}",
                "ERP Application Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void AddPurchaseInvoiceButton_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel == null)
        {
            return;
        }

        var item = ResolveItem();
        var supplier = _viewModel.Suppliers.FirstOrDefault();

        if (item == null)
        {
            StatusTextBlock.Text = "No active items are available.";
            return;
        }

        if (supplier == null)
        {
            StatusTextBlock.Text = "No supplier is available.";
            return;
        }

        var invoice = new PurchaseInvoice
        {
            Number = string.IsNullOrWhiteSpace(InvoiceNumberTextBox.Text) ? null! : InvoiceNumberTextBox.Text.Trim(),
            SupplierId = supplier.Id,
            Notes = "Created from the internal screen",
            TotalAmount = item.PurchasePrice * GetQuantity(),
            Discount = 0,
            PaymentMethod = "Cash",
            Items = new List<PurchaseInvoiceItem>
            {
                new()
                {
                    ItemId = item.Id,
                    Quantity = GetQuantity(),
                    UnitPrice = item.PurchasePrice
                }
            }
        };

        try
        {
            await _viewModel.CreatePurchaseInvoiceAsync(invoice);
            StatusTextBlock.Text = $"Purchase invoice {invoice.Number} created.";
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message, "ERP Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void AddSaleInvoiceButton_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel == null)
        {
            return;
        }

        var item = ResolveItem();
        var customer = _viewModel.Customers.FirstOrDefault();

        if (item == null)
        {
            StatusTextBlock.Text = "No active items are available.";
            return;
        }

        if (customer == null)
        {
            StatusTextBlock.Text = "No customer is available.";
            return;
        }

        var invoice = new SaleInvoice
        {
            Number = string.IsNullOrWhiteSpace(InvoiceNumberTextBox.Text) ? null! : InvoiceNumberTextBox.Text.Trim(),
            CustomerId = customer.Id,
            Discount = 0,
            Tax = 0,
            PaymentMethod = "Cash",
            PaymentStatus = "Paid",
            Items = new List<SaleInvoiceItem>
            {
                new()
                {
                    ItemId = item.Id,
                    Quantity = GetQuantity(),
                    UnitPrice = item.SalePrice
                }
            }
        };

        try
        {
            await _viewModel.CreateSaleInvoiceAsync(invoice);
            StatusTextBlock.Text = $"Sale invoice {invoice.Number} created.";
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message, "ERP Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private Item? ResolveItem()
    {
        if (_viewModel == null)
        {
            return null;
        }

        var items = _viewModel.Items;
        if (items.Count == 0)
        {
            return null;
        }

        var input = ItemCodeTextBox.Text.Trim();
        if (!string.IsNullOrWhiteSpace(input))
        {
            return items.FirstOrDefault(item =>
                item.Code.Equals(input, StringComparison.OrdinalIgnoreCase) ||
                item.Name.Contains(input, StringComparison.OrdinalIgnoreCase));
        }

        return items.First();
    }

    private int GetQuantity()
    {
        return int.TryParse(QuantityTextBox.Text, out var quantity) && quantity > 0 ? quantity : 1;
    }
}

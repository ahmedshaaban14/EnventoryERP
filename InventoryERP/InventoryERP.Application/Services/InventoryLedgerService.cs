using InventoryERP.Application.Interfaces;
using InventoryERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryERP.Application.Services;

public class InventoryLedgerService
{
    private readonly IInventoryDbContext _context;

    public InventoryLedgerService(IInventoryDbContext context)
    {
        _context = context;
    }

    public async Task RecordPurchaseAsync(PurchaseInvoice invoice, CancellationToken cancellationToken = default)
    {
        var supplier = await _context.Suppliers.FirstOrDefaultAsync(x => x.Id == invoice.SupplierId, cancellationToken);
        if (supplier is null)
        {
            throw new InvalidOperationException("المورد غير موجود.");
        }

        foreach (var item in invoice.Items)
        {
            var stockItem = await _context.Items.FirstOrDefaultAsync(x => x.Id == item.ItemId, cancellationToken);
            if (stockItem is null)
            {
                continue;
            }

            var before = stockItem.CurrentStock;
            stockItem.CurrentStock += item.Quantity;
            _context.StockMovements.Add(new StockMovement
            {
                ItemId = stockItem.Id,
                MovementType = "شراء",
                QuantityBefore = before,
                QuantityChange = item.Quantity,
                QuantityAfter = stockItem.CurrentStock,
                Source = "PurchaseInvoice"
            });
        }

        supplier.TotalPurchases += invoice.TotalAmount;
        supplier.TotalPayments += invoice.TotalAmount;

        _context.AccountTransactions.Add(new AccountTransaction
        {
            Description = $"مشتريات فاتورة {invoice.Number}",
            ReferenceNumber = invoice.Number,
            Debit = invoice.TotalAmount,
            Credit = 0,
            Balance = 0,
            SupplierId = supplier.Id
        });

        var cashSafe = await _context.CashSafes.FirstOrDefaultAsync(cancellationToken);
        if (cashSafe is not null)
        {
            cashSafe.CurrentBalance -= invoice.TotalAmount;
            _context.CashMovements.Add(new CashMovement
            {
                CashSafeId = cashSafe.Id,
                Reason = $"فاتورة شراء {invoice.Number}",
                Amount = invoice.TotalAmount,
                OperationType = "صرف"
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RecordSaleAsync(SaleInvoice invoice, CancellationToken cancellationToken = default)
    {
        var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == invoice.CustomerId, cancellationToken);
        if (customer is null)
        {
            throw new InvalidOperationException("العميل غير موجود.");
        }

        foreach (var item in invoice.Items)
        {
            var stockItem = await _context.Items.FirstOrDefaultAsync(x => x.Id == item.ItemId, cancellationToken);
            if (stockItem is null)
            {
                continue;
            }

            var before = stockItem.CurrentStock;
            stockItem.CurrentStock -= item.Quantity;
            _context.StockMovements.Add(new StockMovement
            {
                ItemId = stockItem.Id,
                MovementType = "بيع",
                QuantityBefore = before,
                QuantityChange = -item.Quantity,
                QuantityAfter = stockItem.CurrentStock,
                Source = "SaleInvoice"
            });
        }

        customer.TotalSales += invoice.Items.Sum(x => x.Quantity * x.UnitPrice);
        customer.TotalPayments += invoice.Items.Sum(x => x.Quantity * x.UnitPrice);

        _context.AccountTransactions.Add(new AccountTransaction
        {
            Description = $"مبيعات فاتورة {invoice.Number}",
            ReferenceNumber = invoice.Number,
            Debit = 0,
            Credit = invoice.Items.Sum(x => x.Quantity * x.UnitPrice),
            Balance = 0,
            CustomerId = customer.Id
        });

        var cashSafe = await _context.CashSafes.FirstOrDefaultAsync(cancellationToken);
        if (cashSafe is not null)
        {
            cashSafe.CurrentBalance += invoice.Items.Sum(x => x.Quantity * x.UnitPrice);
            _context.CashMovements.Add(new CashMovement
            {
                CashSafeId = cashSafe.Id,
                Reason = $"فاتورة بيع {invoice.Number}",
                Amount = invoice.Items.Sum(x => x.Quantity * x.UnitPrice),
                OperationType = "قبض"
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}

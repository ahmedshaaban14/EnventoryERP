using InventoryERP.Application.Interfaces;
using InventoryERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryERP.Application.Services;

public class InvoiceService : IInvoiceService
{
    // Keep old prototype methods but map them to current interface.
    private readonly IInventoryDbContext _context;

    private readonly InventoryLedgerService _ledgerService;

    public InvoiceService(IInventoryDbContext context)
        : this(context, new InventoryLedgerService(context))
    {
    }

    public InvoiceService(IInventoryDbContext context, InventoryLedgerService ledgerService)
    {
        _context = context;
        _ledgerService = ledgerService;
    }

    public async Task<SaleInvoice> CreateSaleInvoiceAsync(SaleInvoice invoice, CancellationToken cancellationToken = default)

    {
        if (string.IsNullOrWhiteSpace(invoice.Number))
        {
            invoice.Number = await GenerateSaleNumberAsync(cancellationToken);
        }

        if (invoice.Items.Count == 0)
        {
            var firstItem = await _context.Items.Where(x => x.IsActive).OrderBy(x => x.Id).FirstOrDefaultAsync(cancellationToken);
            if (firstItem is not null)
            {
                invoice.Items.Add(new SaleInvoiceItem
                {
                    ItemId = firstItem.Id,
                    Quantity = 1,
                    UnitPrice = firstItem.SalePrice
                });
            }
        }

        _context.SaleInvoices.Add(invoice);
        await _context.SaveChangesAsync(cancellationToken);
        await _ledgerService.RecordSaleAsync(invoice, cancellationToken);
        return invoice;
    }

    public async Task<PurchaseInvoice> CreatePurchaseInvoiceAsync(PurchaseInvoice invoice, CancellationToken cancellationToken = default)

    {
        if (string.IsNullOrWhiteSpace(invoice.Number))
        {
            invoice.Number = await GeneratePurchaseNumberAsync(cancellationToken);
        }

        if (invoice.Items.Count == 0)
        {
            var firstItem = await _context.Items.Where(x => x.IsActive).OrderBy(x => x.Id).FirstOrDefaultAsync(cancellationToken);
            if (firstItem is not null)
            {
                invoice.Items.Add(new PurchaseInvoiceItem
                {
                    ItemId = firstItem.Id,
                    Quantity = 1,
                    UnitPrice = firstItem.LastPurchasePrice

                });
            }
        }

        _context.PurchaseInvoices.Add(invoice);
        await _context.SaveChangesAsync(cancellationToken);
        await _ledgerService.RecordPurchaseAsync(invoice, cancellationToken);
        return invoice;
    }

    public async Task<IReadOnlyList<SaleInvoice>> GetSaleInvoicesAsync(CancellationToken cancellationToken = default)
        => await _context.SaleInvoices.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<PurchaseInvoice>> GetPurchaseInvoicesAsync(CancellationToken cancellationToken = default)
        => await _context.PurchaseInvoices.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<SaleInvoice?> GetSaleInvoiceByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.SaleInvoices.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<PurchaseInvoice?> GetPurchaseInvoiceByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.PurchaseInvoices.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task CancelSaleInvoiceAsync(int id, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task CancelPurchaseInvoiceAsync(int id, CancellationToken cancellationToken = default)
        => Task.CompletedTask;


    private async Task<string> GenerateSaleNumberAsync(CancellationToken cancellationToken)
    {
        var last = await _context.SaleInvoices
            .OrderByDescending(x => x.Id)
            .Select(x => x.Number)
            .FirstOrDefaultAsync(cancellationToken);

        var number = 1;
        if (!string.IsNullOrWhiteSpace(last) && last.StartsWith("SAL-", StringComparison.OrdinalIgnoreCase))
        {
            var parsed = int.TryParse(last.Replace("SAL-", string.Empty), out var n);
            if (parsed) number = n + 1;
        }

        return $"SAL-{number:D6}";

    }

    private async Task<string> GeneratePurchaseNumberAsync(CancellationToken cancellationToken)
    {
        var last = await _context.PurchaseInvoices
            .OrderByDescending(x => x.Id)
            .Select(x => x.Number)
            .FirstOrDefaultAsync(cancellationToken);

        var number = 1;
        if (!string.IsNullOrWhiteSpace(last) && last.StartsWith("PUR-", StringComparison.OrdinalIgnoreCase))
        {
            var parsed = int.TryParse(last.Replace("PUR-", string.Empty), out var n);
            if (parsed) number = n + 1;
        }

        return $"PUR-{number:D6}";
    }
}

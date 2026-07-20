using InventoryERP.Domain.Entities;

namespace InventoryERP.Application.Interfaces;

public interface IInvoiceService
{
    Task<IReadOnlyList<SaleInvoice>> GetSaleInvoicesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PurchaseInvoice>> GetPurchaseInvoicesAsync(CancellationToken cancellationToken = default);
    Task<SaleInvoice?> GetSaleInvoiceByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PurchaseInvoice?> GetPurchaseInvoiceByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<SaleInvoice> CreateSaleInvoiceAsync(SaleInvoice invoice, CancellationToken cancellationToken = default);
    Task<PurchaseInvoice> CreatePurchaseInvoiceAsync(PurchaseInvoice invoice, CancellationToken cancellationToken = default);
    Task CancelSaleInvoiceAsync(int id, CancellationToken cancellationToken = default);
    Task CancelPurchaseInvoiceAsync(int id, CancellationToken cancellationToken = default);
}

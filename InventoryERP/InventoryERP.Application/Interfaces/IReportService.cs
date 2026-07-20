using InventoryERP.Application.Common;

namespace InventoryERP.Application.Interfaces;

public interface IReportService
{
    Task<InventoryReport> GetInventoryReportAsync(CancellationToken cancellationToken = default);
}

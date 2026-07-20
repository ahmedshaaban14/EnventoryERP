using InventoryERP.Application.Common;

namespace InventoryERP.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardOverview> GetDashboardAsync(CancellationToken cancellationToken = default);
}

namespace InventoryERP.Application.Common;

public class DashboardSummary
{
    public int TotalItems { get; set; }
    public decimal CurrentInventoryValue { get; set; }
    public decimal MonthlySales { get; set; }
    public decimal MonthlyPurchases { get; set; }
    public int LowStockItems { get; set; }
    public decimal TotalCustomerReceivables { get; set; }
    public decimal TotalSupplierPayables { get; set; }
    public decimal CashBalance { get; set; }
}

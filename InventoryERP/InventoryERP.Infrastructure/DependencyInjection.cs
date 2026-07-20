using InventoryERP.Application.Interfaces;
using InventoryERP.Application.Services;


using InventoryERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryERP.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<IInventoryDbContext, InventoryDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IItemService, ItemService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<IInvoiceService, InvoiceService>();


        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<InventoryLedgerService>();
        return services;
    }
}

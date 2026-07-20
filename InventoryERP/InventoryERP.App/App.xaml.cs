﻿﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using InventoryERP.Domain.Entities;
using InventoryERP.Infrastructure;
using InventoryERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace InventoryERP.App;

public partial class InventoryApp : System.Windows.Application
{
    public static IHost? AppHost { get; private set; }

    private static readonly object _logLock = new();
    private static readonly string LogFilePath = Path.Combine(AppContext.BaseDirectory, "testdiag.log");

    protected override void OnStartup(StartupEventArgs e)
    {
        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        // Fail fast & collect diagnostics for any crash.
        DispatcherUnhandledException += (_, args) =>
        {
            LogException(args.Exception, "DispatcherUnhandledException");
            ShowFriendlyError(args.Exception, "حدث خطأ غير متوقع في الواجهة.");
            args.Handled = true;
        };

        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            if (args.ExceptionObject is Exception ex)
            {
                LogException(ex, "AppDomain.UnhandledException");
                ShowFriendlyError(ex, "حدث خطأ غير متوقع في التطبيق.");
            }
        };

        TaskScheduler.UnobservedTaskException += (_, args) =>
        {
            LogException(args.Exception, "TaskScheduler.UnobservedTaskException");
            ShowFriendlyError(args.Exception, "حدث خطأ غير متوقع أثناء تنفيذ مهمة خلفية.");
            args.SetObserved();
        };

        base.OnStartup(e);

        _ = StartupInternalAsync(e);
    }

    private static async Task StartupInternalAsync(StartupEventArgs e)
    {
        try
        {
            AppHost = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    var connectionString = "Data Source=inventoryerp.db";

                    services.AddInfrastructure(connectionString);

                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<INavigationService, NavigationService>();
                    services.AddTransient<MainViewModel>();


                    services.AddTransient<ItemsViewModel>();
                    services.AddTransient<CustomersViewModel>();
                    services.AddTransient<SuppliersViewModel>();
                    services.AddTransient<InvoicesViewModel>();
                    services.AddTransient<CashViewModel>();
                    services.AddTransient<ReportsViewModel>();
                })
                .Build();

            await using var scope = AppHost.Services.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();

            await EnsureDatabaseSchemaAsync(dbContext);
            await SeedDataAsync(dbContext);


            var mainWindow = AppHost.Services.GetRequiredService<MainWindow>();
            Current.MainWindow = mainWindow;
            mainWindow.Show();
            Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
        }
        catch (Exception ex)
        {
            LogException(ex, "StartupException");
            ShowFriendlyError(ex, "تعذر بدء التطبيق بشكل صحيح.");
            Current.Shutdown(-1);
        }
    }

    private static void LogException(Exception ex, string kind)
    {
        try
        {
            var sb = new StringBuilder();
            sb.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {kind}");
            sb.AppendLine($"Type: {ex.GetType().FullName}");
            sb.AppendLine($"Message: {ex.Message}");
            sb.AppendLine("StackTrace:");
            sb.AppendLine(ex.ToString());

            if (ex.InnerException != null)
            {
                sb.AppendLine("InnerException:");
                sb.AppendLine(ex.InnerException.ToString());
            }

            sb.AppendLine(new string('-', 80));

            lock (_logLock)
            {
                File.AppendAllText(LogFilePath, sb.ToString());
            }
        }
        catch
        {
            // Logging must never hide the original exception.
        }
    }

    private static void ShowFriendlyError(Exception ex, string message)
    {
        MessageBox.Show(
            $"{message}{Environment.NewLine}{Environment.NewLine}{ex.Message}",
            "ERP Application Error",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }

    private static async Task EnsureDatabaseSchemaAsync(InventoryDbContext dbContext)
    {
        // Strategy:
        // 1) If migrations exist, try to migrate.
        // 2) If the DB is incompatible/migrations are missing, back up and recreate from current model.

        var connectionString = dbContext.Database.GetDbConnection().ConnectionString;
        var dbFileName = "inventoryerp.db";

        try
        {
            // If migrations are configured, prefer them.
            // Will throw if migrations assembly is missing/broken or provider can’t apply.
            await dbContext.Database.MigrateAsync();
            return;
        }
        catch (Exception migrateEx)
        {
            LogException(migrateEx, "DatabaseMigrateFailed");

            // Try EnsureCreated first (covers cases where there are no migrations).
            try
            {
                await dbContext.Database.EnsureCreatedAsync();

                // Validate that at least the core lookup tables exist.
                // If not, the existing DB file is incompatible and we need to recreate.
                if (!await dbContext.Categories.AnyAsync())
                {
                    // If Categories table doesn’t exist, AnyAsync will throw and be caught below.
                    // If it exists but empty, that’s fine.
                }

                return;
            }
            catch (Exception ensureEx)
            {
                LogException(ensureEx, "DatabaseEnsureCreatedFailed");

                // Recreate clean DB: backup old file and delete.
                try
                {
                    var dbPath = GetSqliteDbPathFromConnectionString(connectionString) ?? Path.Combine(AppContext.BaseDirectory, dbFileName);

                    if (File.Exists(dbPath))
                    {
                        var backupPath = $"{dbPath}.{DateTime.Now:yyyyMMdd_HHmmss}.bak";
                        File.Copy(dbPath, backupPath, overwrite: true);
                        File.Delete(dbPath);

                        // Remove WAL/SHM if present.
                        var walPath = dbPath + "-wal";
                        var shmPath = dbPath + "-shm";
                        if (File.Exists(walPath)) File.Delete(walPath);
                        if (File.Exists(shmPath)) File.Delete(shmPath);
                    }
                }
                catch (Exception recreateBackupEx)
                {
                    LogException(recreateBackupEx, "DatabaseBackupRecreateFailed");
                    throw; // don’t hide exception
                }

                // Create schema from current model.
                await dbContext.Database.EnsureCreatedAsync();
            }
        }
    }

    private static string? GetSqliteDbPathFromConnectionString(string connectionString)
    {
        // Typical: "Data Source=inventoryerp.db"
        var parts = connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries);
        foreach (var part in parts)
        {
            var trimmed = part.Trim();
            if (trimmed.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase))
            {
                return trimmed.Substring("Data Source=".Length).Trim(' ', '"');
            }
        }
        return null;
    }

    private static async Task SeedDataAsync(InventoryDbContext dbContext)
    {
        if (await dbContext.Items.AnyAsync())
        {
            return;
        }


        var items = new List<Item>
        {
            new() { Code = "ITM-001", Name = "فلتر زيت", CategoryId = 1, Brand = "Bosch", Model = "C-Class", Barcode = "111111", PurchasePrice = 25, SalePrice = 40, CurrentStock = 15, MinimumStock = 5, Unit = "قطعة", Rack = "A-01", Description = "فلتر زيت للسيارات" },
            new() { Code = "ITM-002", Name = "شمعات احتراق", CategoryId = 2, Brand = "NGK", Model = "Accord", Barcode = "222222", PurchasePrice = 18, SalePrice = 30, CurrentStock = 8, MinimumStock = 4, Unit = "قطعة", Rack = "A-02", Description = "شمعات احتراق" },
            new() { Code = "ITM-003", Name = "إطارات أمامية", CategoryId = 3, Brand = "Michelin", Model = "Corolla", Barcode = "333333", PurchasePrice = 320, SalePrice = 480, CurrentStock = 6, MinimumStock = 3, Unit = "زوج", Rack = "B-01", Description = "إطارات أمامية" }
        };

        var categories = new List<Category>
        {
            new() { Name = "صيانة" },
            new() { Name = "إشعال" },
            new() { Name = "إطارات" }
        };

        dbContext.Categories.AddRange(categories);
        await dbContext.SaveChangesAsync();

        dbContext.Items.AddRange(items);

        var suppliers = new List<Supplier>
        {
            new() { Code = "SUP-001", Name = "مورد السيارات", Phone = "01000000000", Address = "القاهرة", OpeningBalance = 0, TotalPurchases = 5000, TotalPayments = 1000 },
            new() { Code = "SUP-002", Name = "مورد الإطارات", Phone = "01111111111", Address = "الجيزة", OpeningBalance = 0, TotalPurchases = 3000, TotalPayments = 500 }
        };

        dbContext.Suppliers.AddRange(suppliers);

        var customers = new List<Customer>
        {
            new() { Code = "CST-001", Name = "أحمد علي", Phone = "01200000000", Address = "الأسكندرية", OpeningBalance = 0, TotalSales = 3000, TotalPayments = 1000, CreditLimit = 5000 },
            new() { Code = "CST-002", Name = "سامي محمد", Phone = "01300000000", Address = "المنصورة", OpeningBalance = 0, TotalSales = 2000, TotalPayments = 500, CreditLimit = 4000 }
        };

        dbContext.Customers.AddRange(customers);

        var cashSafe = new CashSafe { Name = "الخزنة الأساسية", CurrentBalance = 15000 };
        dbContext.CashSafes.Add(cashSafe);

        await dbContext.SaveChangesAsync();
    }
}


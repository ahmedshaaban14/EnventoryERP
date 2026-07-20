# TODO - Database schema alignment + startup fix

- [ ] Inspect EF Core project for migrations usage (if any) and confirm DbContext configuration.
- [ ] Implement startup logic in `InventoryERP.App/App.xaml.cs`:
  - [ ] Try `context.Database.Migrate()` (if migrations exist).
  - [ ] On failure/incompatible schema: backup existing SQLite DB, delete it, recreate schema from current EF Core models.
  - [ ] Ensure exceptions are not hidden.
- [ ] Ensure `InventoryDbContext.OnModelCreating` config covers missing FK/index/column expectations so schema matches models.
- [ ] Expand seeding so required lookup data exists for all navigation pages.
- [ ] Build solution.
- [ ] Run app and verify all pages open successfully without SQLite exceptions:
  - [ ] Dashboard
  - [ ] Items
  - [ ] Customers
  - [ ] Suppliers
  - [ ] Purchase Invoices
  - [ ] Sales Invoices
  - [ ] Reports
  - [ ] Inventory


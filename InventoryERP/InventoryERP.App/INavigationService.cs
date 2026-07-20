namespace InventoryERP.App;

public interface INavigationService
{
    object Navigate(AppPage page);
    object? GetPage(AppPage page);
}

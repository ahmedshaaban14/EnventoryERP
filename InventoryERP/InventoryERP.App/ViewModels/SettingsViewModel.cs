using CommunityToolkit.Mvvm.ComponentModel;

namespace InventoryERP.App;

public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private string applicationName = "Inventory ERP";

    [ObservableProperty]
    private string companyName = "Inventory ERP";

    [ObservableProperty]
    private bool darkModeEnabled;

    [ObservableProperty]
    private string backupFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
}

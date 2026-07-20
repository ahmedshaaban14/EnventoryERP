using CommunityToolkit.Mvvm.ComponentModel;

namespace InventoryERP.App;

public partial class NavigationItemViewModel : ObservableObject
{
    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private string icon = string.Empty;

    [ObservableProperty]
    private AppPage page;

    [ObservableProperty]
    private bool isSelected;
}

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventoryERP.Application.Interfaces;
using InventoryERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace InventoryERP.App;

public partial class ItemsViewModel : ObservableObject
{
    private readonly IItemService _itemService;
    private readonly ICategoryService _categoryService;
    private readonly List<Item> _allItems = new();
    private CancellationTokenSource? _searchCts;

    [ObservableProperty]
    private ObservableCollection<Item> items = new();

    [ObservableProperty]
    private ObservableCollection<Category> categories = new();

    [ObservableProperty]
    private Item? selectedItem;

    [ObservableProperty]
    private Item editorItem = CreateEditorItem();

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool isNoItemsFound;

    public ItemsViewModel(IItemService itemService, ICategoryService categoryService)
    {
        _itemService = itemService;
        _categoryService = categoryService;
    }

    public async Task LoadAsync()
    {
        IsBusy = true;

        try
        {
            var categories = await _categoryService.GetAllAsync();
            Categories = new ObservableCollection<Category>(categories);

            var items = await _itemService.GetAllAsync();
            _allItems.Clear();
            _allItems.AddRange(items);

            await ApplyFilterAsync(SearchText, CancellationToken.None);

            if (SelectedItem == null)
            {
                EditorItem = CreateEditorItem();
            }

            StatusMessage = $"تم تحميل {Items.Count} صنف";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void NewItem()
    {
        SelectedItem = null;
        EditorItem = CreateEditorItem();
        StatusMessage = "جاهز لإضافة صنف جديد";
    }

    [RelayCommand]
    private async Task SaveItemAsync()
    {
        ValidateEditorItem();

        if (EditorItem.Id == 0)
        {
            await _itemService.AddAsync(EditorItem);
            StatusMessage = "تمت إضافة الصنف بنجاح";
        }
        else
        {
            await _itemService.UpdateAsync(EditorItem);
            StatusMessage = "تم تحديث الصنف بنجاح";
        }

        var savedCode = EditorItem.Code;
        await LoadAsync();
        SelectedItem = Items.FirstOrDefault(item => item.Code == savedCode);
    }

    [RelayCommand]
    private async Task DeleteItemAsync()
    {
        if (SelectedItem == null)
        {
            return;
        }

        await _itemService.DeleteAsync(SelectedItem.Id);
        StatusMessage = "تم حذف الصنف";
        SelectedItem = null;
        EditorItem = CreateEditorItem();
        await LoadAsync();
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadAsync();
    }

    [RelayCommand]
    private void ClearSearch()
    {
        SearchText = string.Empty;
    }

    partial void OnSelectedItemChanged(Item? value)
    {
        if (value == null)
        {
            EditorItem = CreateEditorItem();
            return;
        }

        EditorItem = CloneItem(value);
    }

    partial void OnSearchTextChanged(string value)
    {
        ScheduleFilter();
    }

    private void ScheduleFilter()
    {
        _searchCts?.Cancel();
        _searchCts?.Dispose();
        _searchCts = new CancellationTokenSource();
        var cancellationToken = _searchCts.Token;

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(250, cancellationToken);
                await ApplyFilterAsync(SearchText, cancellationToken);
            }
            catch (OperationCanceledException)
            {
            }
        }, CancellationToken.None);
    }

    private async Task ApplyFilterAsync(string searchText, CancellationToken cancellationToken)
    {
        var query = (searchText ?? string.Empty).Trim();
        var comparison = StringComparison.OrdinalIgnoreCase;

        var filtered = string.IsNullOrWhiteSpace(query)
            ? _allItems.ToList()
            : _allItems.Where(item =>
                    (item.Code ?? string.Empty).Contains(query, comparison) ||
                    (item.Barcode ?? string.Empty).Contains(query, comparison) ||
                    (item.Name ?? string.Empty).Contains(query, comparison) ||
                    (item.Brand ?? string.Empty).Contains(query, comparison) ||
                    (item.Model ?? string.Empty).Contains(query, comparison) ||
                    (item.StorageLocation ?? string.Empty).Contains(query, comparison) ||
                    (item.Category?.Name ?? string.Empty).Contains(query, comparison))
                .ToList();

        await SetItemsOnUiThreadAsync(filtered, cancellationToken);
    }

    private Task SetItemsOnUiThreadAsync(List<Item> items, CancellationToken cancellationToken)
    {
        var dispatcher = System.Windows.Application.Current?.Dispatcher;
        if (dispatcher == null || dispatcher.CheckAccess())
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                Items = new ObservableCollection<Item>(items);
                IsNoItemsFound = Items.Count == 0;
            }

            return Task.CompletedTask;
        }

        return dispatcher.InvokeAsync(() =>
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                Items = new ObservableCollection<Item>(items);
                IsNoItemsFound = Items.Count == 0;
            }
        }, System.Windows.Threading.DispatcherPriority.Background, cancellationToken).Task;
    }

    private static Item CreateEditorItem()
    {
        return new Item
        {
            Unit = "قطعة",
            IsActive = true
        };
    }

    private static Item CloneItem(Item item)
    {
        return new Item
        {
            Id = item.Id,
            Code = item.Code,
            Barcode = item.Barcode,
            Name = item.Name,
            EnglishName = item.EnglishName,
            CategoryId = item.CategoryId,
            Brand = item.Brand,
            Car = item.Car,
            Model = item.Model,
            Year = item.Year,
            Unit = item.Unit,
            MinimumStock = item.MinimumStock,
            Rack = item.Rack,
            Shelf = item.Shelf,
            LastPurchasePrice = item.LastPurchasePrice,
            AverageCost = item.AverageCost,
            SalePrice = item.SalePrice,
            WholesalePrice = item.WholesalePrice,
            CurrentStock = item.CurrentStock,
            ImagePath = item.ImagePath,
            Notes = item.Notes,
            IsActive = item.IsActive
        };
    }

    private void ValidateEditorItem()
    {
        if (string.IsNullOrWhiteSpace(EditorItem.Name))
        {
            throw new InvalidOperationException("اسم الصنف مطلوب.");
        }

        if (EditorItem.SalePrice < 0 || EditorItem.WholesalePrice < 0 || EditorItem.LastPurchasePrice < 0)
        {
            throw new InvalidOperationException("الأسعار يجب أن تكون أكبر من أو تساوي صفر.");
        }

        if (EditorItem.MinimumStock < 0)
        {
            throw new InvalidOperationException("الحد الأدنى لا يمكن أن يكون سالبًا.");
        }
    }
}

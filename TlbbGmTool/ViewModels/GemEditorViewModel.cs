using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.Models;
using liuguang.TlbbGmTool.Services;
using liuguang.TlbbGmTool.ViewModels.Data;
using liuguang.TlbbGmTool.Views.Item;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace liuguang.TlbbGmTool.ViewModels;
public class GemEditorViewModel : ViewModelBase
{
    #region Fields
    private bool _isSaving = false;
    private ItemLogViewModel? _inputItemLog;
    private GemDataViewModel _itemData = new();
    private BagContainer? _itemsContainer;
    /// <summary>
    /// 数据库连接
    /// </summary>
    public DbConnection? Connection;
    #endregion
    #region Properties
    public bool IsSaving
    {
        get => _isSaving;
        set
        {
            if (SetProperty(ref _isSaving, value))
            {
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
    }
    public string WindowTitle
    {
        get
        {
            if (_inputItemLog is null)
            {
                return "发放宝石";
            }
            return "修改宝石 " + _itemData.ItemName;
        }
    }
    public ItemLogViewModel ItemLog
    {
        set
        {
            _inputItemLog = value;
            GemDataService.Read(value.ItemBaseId, value.PData, _itemData);
        }
    }
    public BagContainer ItemsContainer
    {
        set
        {
            _itemsContainer = value;
            var defaultItem = (from itemBaseInfo in SharedData.ItemBaseMap.Values
                               where itemBaseInfo.TClass == 5
                               select new ItemBaseViewModel(itemBaseInfo)).FirstOrDefault();
            if (defaultItem != null)
            {
                LoadNewItemBase(defaultItem);
            }
        }
    }
    public GemDataViewModel ItemData => _itemData;
    #endregion

    #region Commands
    public Command SelectItemCommand { get; }
    public Command SaveCommand { get; }
    #endregion

    public GemEditorViewModel()
    {
        SelectItemCommand = new(ShowSelectItemWindow);
        SaveCommand = new(SaveItem, () => !_isSaving);
        _itemData.PropertyChanged += ItemData_PropertyChanged;
    }

    /// <summary>
    /// 当选择一个新的宝石id时，初始化对应的数据
    /// </summary>
    /// <param name="itemBase"></param>
    private void LoadNewItemBase(ItemBaseViewModel itemBase)
    {
        _itemData.ItemBaseId = itemBase.ItemBaseId;
        _itemData.RulerId = itemBase.RulerId;
        if (itemBase.BaseInfo is ItemBaseGem gemBaseInfo)
        {
            _itemData.BasePrice = gemBaseInfo.BasePrice;
            _itemData.AttrType = gemBaseInfo.AttrType;
            _itemData.AttrValue = gemBaseInfo.AttrValue;
        }
    }

    private void ItemData_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_itemData.ItemName))
        {
            RaisePropertyChanged(nameof(WindowTitle));
        }
    }

    /// <summary>
    /// 展示修改物品id的窗体
    /// </summary>
    private void ShowSelectItemWindow()
    {
        var selectorWindow = new LvItemSelectorWindow();
        var beforeAction = (LvItemSelectorViewModel vm) =>
        {
            vm.WindowTitle = "选择宝石";
            vm.InitItemId = _itemData.ItemBaseId;
            vm.ItemList = (from itemBaseInfo in SharedData.ItemBaseMap.Values
                           where itemBaseInfo.TClass == 5
                           select new ItemBaseViewModel(itemBaseInfo)).ToList();
        };
        if (ShowDialog(selectorWindow, beforeAction) == true)
        {
            var selectedItem = selectorWindow.SelectedItem;
            if (selectedItem != null)
            {
                LoadNewItemBase(selectedItem);
            }
        }
    }

    private async void SaveItem()
    {
        if (Connection is null)
        {
            return;
        }
        var itemBaseId = _itemData.ItemBaseId;
        byte[] pData = new byte[17 * 4];
        GemDataService.Write(_itemData, pData);
        if (_inputItemLog is null)
        {
            await InsertItemAsync(Connection, itemBaseId, pData);
        }
        else
        {
            await UpdateItemAsync(Connection, _inputItemLog, itemBaseId, pData);
        }
    }


    private async Task InsertItemAsync(DbConnection connection, int itemBaseId, byte[] pData)
    {
        if (_itemsContainer is null)
        {
            return;
        }
        var serverType = connection.GameServerType;
        ItemLogViewModel itemLog = new(new()
        {
            CharGuid = _itemsContainer.CharGuid,
            ItemBaseId = itemBaseId,
            PData = pData,
        }, serverType);
        try
        {
            await Task.Run(async () =>
            {
                await ItemDbService.InsertItemAsync(connection, _itemsContainer.PosOffset, _itemsContainer.BagMaxSize, itemLog);
            });
            _itemsContainer.InsertNewItem(itemLog);
            ShowMessage("发放成功", $"发放宝石成功,pos={itemLog.Pos}");
            OwnedWindow?.Close();
        }
        catch (Exception ex)
        {
            ShowErrorMessage("发放失败", ex, true);
        }
        finally
        {
            IsSaving = false;
        }
    }

    private async Task UpdateItemAsync(DbConnection connection, ItemLogViewModel itemLog, int itemBaseId, byte[] pData)
    {
        try
        {
            await Task.Run(async () =>
            {
                await ItemDbService.UpdateItemAsync(connection, itemLog.Id, itemBaseId, pData);
            });
            itemLog.ItemBaseId = itemBaseId;
            itemLog.PData = pData;
            ShowMessage("修改成功", "修改宝石成功");
            OwnedWindow?.Close();
        }
        catch (Exception ex)
        {
            ShowErrorMessage("修改失败", ex, true);
        }
        finally
        {
            IsSaving = false;
        }
    }
}

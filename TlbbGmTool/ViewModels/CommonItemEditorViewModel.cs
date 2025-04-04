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
public class CommonItemEditorViewModel : ViewModelBase
{
    #region Fields
    private bool _isSaving = false;
    private ItemLogViewModel? _inputItemLog;
    private CommonItemDataViewModel _itemData = new();
    private BagContainer? _itemsContainer;
    /// <summary>
    /// 数据库连接
    /// </summary>
    public DbConnection? Connection;
    public BagType RoleBagType = BagType.ItemBag;
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
                return "发放物品";
            }
            return "修改物品 " + _itemData.ItemName;
        }
    }
    public ItemLogViewModel ItemLog
    {
        set
        {
            _inputItemLog = value;
            CommonItemDataService.Read(value.ItemBaseId, value.PData, _itemData);
        }
    }
    public BagContainer ItemsContainer
    {
        set
        {
            _itemsContainer = value;
            var filterClass = 3;
            switch (RoleBagType)
            {
                case BagType.ItemBag:
                    filterClass = 3;
                    break;
                case BagType.MaterialBag:
                    filterClass = 2;
                    break;
                case BagType.TaskBag:
                    filterClass = 4;
                    break;

            }
            var defaultItem = (from itemBaseInfo in SharedData.ItemBaseMap.Values
                               where itemBaseInfo.TClass == filterClass
                               select new ItemBaseViewModel(itemBaseInfo)).FirstOrDefault();
            if (defaultItem != null)
            {
                LoadNewItemBase(defaultItem);
            }
        }
    }
    public CommonItemDataViewModel ItemData => _itemData;
    /// <summary>
    /// 数量编辑功能的状态
    /// </summary>
    public bool CountEditorEnabled => (_itemData.MaxSize > 1);
    #endregion

    #region Commands
    public Command SelectItemCommand { get; }
    public Command SaveCommand { get; }
    #endregion

    public CommonItemEditorViewModel()
    {
        SelectItemCommand = new(ShowSelectItemWindow);
        SaveCommand = new(SaveItem, () => !_isSaving);
        _itemData.PropertyChanged += ItemData_PropertyChanged;
    }

    /// <summary>
    /// 当选择一个新的物品id时，初始化对应的数据
    /// </summary>
    /// <param name="itemBase"></param>
    private void LoadNewItemBase(ItemBaseViewModel itemBase)
    {
        _itemData.ItemBaseId = itemBase.ItemBaseId;
        var itemMaxSize = itemBase.ItemMaxSize;
        _itemData.Count = Math.Min(_itemData.Count, itemMaxSize);
        _itemData.MaxSize = itemMaxSize;
        if (itemBase.BaseInfo is ItemBaseCommonItem itemBaseInfo)
        {
            _itemData.RulerId = itemBaseInfo.RulerId;
            _itemData.CosSelf = itemBaseInfo.CosSelf;
            _itemData.BasePrice = itemBaseInfo.BasePrice;
            _itemData.Level = itemBaseInfo.Level;
            _itemData.ReqSkill = itemBaseInfo.ReqSkill;
            _itemData.ReqSkillLevel = itemBaseInfo.ReqSkillLevel;
            _itemData.ScriptID = itemBaseInfo.ScriptID;
            _itemData.SkillID = itemBaseInfo.SkillID;
            _itemData.TargetType = itemBaseInfo.TargetType;
        }
    }

    private void ItemData_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_itemData.ItemName))
        {
            RaisePropertyChanged(nameof(WindowTitle));
        }
        else if (e.PropertyName == nameof(_itemData.MaxSize))
        {
            RaisePropertyChanged(nameof(CountEditorEnabled));
        }
    }

    /// <summary>
    /// 展示修改物品id的窗体
    /// </summary>
    private void ShowSelectItemWindow()
    {
        var selectorWindow = new ItemSelectorWindow();
        var beforeAction = (ItemSelectorViewModel vm) =>
        {
            vm.WindowTitle = "选择物品";
            vm.InitItemId = _itemData.ItemBaseId;
            var filterClass = 3;
            switch (RoleBagType)
            {
                case BagType.ItemBag:
                    filterClass = 3;
                    break;
                case BagType.MaterialBag:
                    filterClass = 2;
                    break;
                case BagType.TaskBag:
                    filterClass = 4;
                    break;

            }
            vm.ItemList = (from itemBaseInfo in SharedData.ItemBaseMap.Values
                           where itemBaseInfo.TClass == filterClass
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
        if ((_itemData.Count < 1) || (_itemData.Count > _itemData.MaxSize))
        {
            ShowErrorMessage("数量不正确", "当前数量设置不正确");
            return;
        }
        if (Connection is null)
        {
            return;
        }
        IsSaving = true;
        var itemBaseId = _itemData.ItemBaseId;
        byte[] pData = new byte[17 * 4];
        CommonItemDataService.Write(_itemData, pData);
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
        ItemLogViewModel itemLog = new(new()
        {
            CharGuid = _itemsContainer.CharGuid,
            ItemBaseId = itemBaseId,
            PData = pData,
        });
        try
        {
            await Task.Run(async () =>
            {
                await ItemDbService.InsertItemAsync(connection, _itemsContainer.PosOffset, _itemsContainer.BagMaxSize, itemLog);
            });
            _itemsContainer.InsertNewItem(itemLog);
            ShowMessage("发放成功", $"发放物品成功,pos={itemLog.Pos}");
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
            ShowMessage("修改成功", "修改物品成功");
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

using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.Models;
using liuguang.TlbbGmTool.Services;
using liuguang.TlbbGmTool.ViewModels.Data;
using liuguang.TlbbGmTool.Views.Item;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace liuguang.TlbbGmTool.ViewModels;
public class EquipEditorViewModel : ViewModelBase
{
    #region Fields
    private bool _isSaving = false;
    private ItemLogViewModel? _inputItemLog;
    private EquipDataViewModel _equipData = new();
    private ObservableCollection<ItemLogViewModel>? _itemList;
    /// <summary>
    /// 发放需要,角色id
    /// </summary>
    public int CharGuid;
    /// <summary>
    /// 发放需要,背包开始位置
    /// </summary>
    public int PosOffset = -1;
    /// <summary>
    /// 发放需要,当前页背包最大容量
    /// </summary>
    public int BagMaxSize = 30;
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
                return "发放装备";
            }
            return "修改装备 " + _equipData.EquipName;
        }
    }
    public ItemLogViewModel ItemLog
    {
        set
        {
            _inputItemLog = value;
            EquipDataService.Read(value.ItemBaseId, value.PData, _equipData);
        }
    }
    public ObservableCollection<ItemLogViewModel> ItemList
    {
        set
        {
            _itemList = value;
            var defaultItem = (from itemBaseInfo in SharedData.ItemBaseMap.Values
                               where itemBaseInfo.TClass == 1
                               select new ItemBaseViewModel(itemBaseInfo)).FirstOrDefault();
            if (defaultItem != null)
            {
                LoadNewItemBase(defaultItem);
            }
        }
    }
    public EquipDataViewModel EquipData => _equipData;
    /// <summary>
    /// 星级选择
    /// </summary>
    public List<ComboBoxNode<byte>> StarSection { get; } = new();
    /// <summary>
    /// 孔数选择
    /// </summary>
    public List<ComboBoxNode<byte>> GemMaxCountSection { get; } = new();
    #endregion

    #region Commands
    public Command SelectEquipCommand { get; }
    public Command SelectVisualCommand { get; }
    public Command SelectGem0Command { get; }
    public Command SelectGem1Command { get; }
    public Command SelectGem2Command { get; }
    public Command SelectGem3Command { get; }
    public Command SelectAttrCommand { get; }
    public Command SaveCommand { get; }
    #endregion

    public EquipEditorViewModel()
    {
        SelectEquipCommand = new(ShowSelectEquipWindow);
        SelectVisualCommand = new(ShowSelectVisualWindow);
        SelectGem0Command = new(() => ShowSelectGemWindow(0), () => _equipData.GemMaxCount > 0);
        SelectGem1Command = new(() => ShowSelectGemWindow(1), () => _equipData.GemMaxCount > 1);
        SelectGem2Command = new(() => ShowSelectGemWindow(2), () => _equipData.GemMaxCount > 2);
        SelectGem3Command = new(() => ShowSelectGemWindow(3), () => _equipData.GemMaxCount > 3);
        SelectAttrCommand = new(ShowSelectAttrWindow, () => _equipData.CanSelectAttr);
        SaveCommand = new(SaveItem, () => !_isSaving);
        for (byte i = 0; i <= 9; i++)
        {
            StarSection.Add(new($"{i}星", i));
        }
        for (byte i = 0; i <= 4; i++)
        {
            GemMaxCountSection.Add(new($"{i}孔", i));
        }
        _equipData.PropertyChanged += EquipData_PropertyChanged;
    }

    /// <summary>
    /// 当选择一个新的装备id时，初始化对应的数据
    /// </summary>
    /// <param name="itemBase"></param>
    private void LoadNewItemBase(ItemBaseViewModel itemBase)
    {
        _equipData.ItemBaseId = itemBase.ItemBaseId;
        //无属性的装备
        if (!_equipData.CanSelectAttr)
        {
            _equipData.Attr0 = 0;
            _equipData.Attr1 = 0;
        }
        if (itemBase.BaseInfo is ItemBaseEquip equipBaseInfo)
        {
            _equipData.RulerId = equipBaseInfo.RulerId;
            _equipData.CurDurPoint = equipBaseInfo.MaxDurPoint;
            _equipData.CurDamagePoint = 0;
            _equipData.MaxDurPoint = equipBaseInfo.MaxDurPoint;
            _equipData.VisualId = equipBaseInfo.EquipVisual;
        }
    }

    private void EquipData_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        //当孔数变化后,通知选择宝石按钮的状态更新
        if (e.PropertyName == nameof(_equipData.GemMaxCount))
        {
            SelectGem0Command.RaiseCanExecuteChanged();
            SelectGem1Command.RaiseCanExecuteChanged();
            SelectGem2Command.RaiseCanExecuteChanged();
            SelectGem3Command.RaiseCanExecuteChanged();
        }
        else if (e.PropertyName == nameof(_equipData.EquipName))
        {
            RaisePropertyChanged(nameof(WindowTitle));
        }
        else if (e.PropertyName == nameof(_equipData.CanSelectAttr))
        {
            SelectAttrCommand.RaiseCanExecuteChanged();
        }
    }

    /// <summary>
    /// 展示修改装备id的窗体
    /// </summary>
    private void ShowSelectEquipWindow()
    {
        var selectorWindow = new ItemSelectorWindow();
        var beforeAction = (ItemSelectorViewModel vm) =>
        {
            vm.WindowTitle = "选择装备";
            vm.InitItemId = _equipData.ItemBaseId;
            vm.ItemList = (from itemBaseInfo in SharedData.ItemBaseMap.Values
                           where itemBaseInfo.TClass == 1
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

    /// <summary>
    /// 展示修改外形的窗体
    /// </summary>
    private void ShowSelectVisualWindow()
    {
        var selectorWindow = new ItemSelectorWindow();
        var visualItemId = _equipData.ParseVisualItemId(_equipData.VisualId);
        var beforeAction = (ItemSelectorViewModel vm) =>
        {
            vm.WindowTitle = "选择外形";
            vm.InitItemId = visualItemId ?? 0;
            vm.ItemList = (from itemBaseInfo in SharedData.ItemBaseMap.Values
                           where itemBaseInfo.TClass == 1
                           let equipBaseInfo = (ItemBaseEquip)itemBaseInfo
                           //更换外观时,装备位置限制必须相同
                           where equipBaseInfo.EquipPoint == _equipData.EquipPoint
                           select new ItemBaseViewModel(itemBaseInfo)).ToList();
        };
        if (ShowDialog(selectorWindow, beforeAction) == true)
        {
            var selectedItem = selectorWindow.SelectedItem;
            if (selectedItem != null)
            {

                if (selectedItem.BaseInfo is ItemBaseEquip equipBaseInfo)
                {
                    _equipData.VisualId = equipBaseInfo.EquipVisual;
                }
            }
        }
    }

    /// <summary>
    /// 展示修改属性种类的窗体
    /// </summary>
    private void ShowSelectAttrWindow()
    {
        var selectorWindow = new AttrSelectorWindow();
        var beforeAction = (AttrSelectorViewModel vm) =>
        {
            if (_equipData.SegAttrs != null)
            {
                vm.EquipValueAttrs = _equipData.SegAttrs;
                vm.Attr0 = _equipData.Attr0;
                vm.Attr1 = _equipData.Attr1;
            }
        };
        if (ShowDialog(selectorWindow, beforeAction) == true)
        {
            _equipData.Attr0 = selectorWindow.Attr0;
            _equipData.Attr1 = selectorWindow.Attr1;
        }
    }
    /// <summary>
    /// 展示选择宝石的窗体
    /// </summary>
    private void ShowSelectGemWindow(int gemIndex)
    {
        int gemId;
        switch (gemIndex)
        {
            case 0:
                gemId = _equipData.Gem0; break;
            case 1:
                gemId = _equipData.Gem1; break;
            case 2:
                gemId = _equipData.Gem2; break;
            case 3:
                gemId = _equipData.Gem3; break;
            default:
                ShowErrorMessage("出错了", $"无效的宝石位置: {gemIndex}");
                return;
        }
        var selectorWindow = new LvItemSelectorWindow();
        var beforeAction = (LvItemSelectorViewModel vm) =>
        {
            vm.WindowTitle = "选择宝石";
            vm.InitItemId = gemId;
            vm.ItemList = (from itemBaseInfo in SharedData.ItemBaseMap.Values
                           where itemBaseInfo.TClass == 5
                           select new ItemBaseViewModel(itemBaseInfo)).ToList();
        };
        if (ShowDialog(selectorWindow, beforeAction) == true)
        {
            var selectedItem = selectorWindow.SelectedItem;
            if (selectedItem != null)
            {
                var selectedGemId = selectedItem.ItemBaseId;
                switch (gemIndex)
                {
                    case 0:
                        _equipData.Gem0 = selectedGemId; break;
                    case 1:
                        _equipData.Gem1 = selectedGemId; break;
                    case 2:
                        _equipData.Gem2 = selectedGemId; break;
                    case 3:
                        _equipData.Gem3 = selectedGemId; break;
                }
            }
        }
    }

    private async void SaveItem()
    {
        if (Connection is null)
        {
            return;
        }
        IsSaving = true;
        var itemBaseId = _equipData.ItemBaseId;
        byte[] pData = new byte[17 * 4];
        if (_inputItemLog is null)
        {
            _equipData.HasCreator = true;
        }
        EquipDataService.Write(_equipData, pData);
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
        ItemLogViewModel itemLog = new(new()
        {
            CharGuid = CharGuid,
            ItemBaseId = itemBaseId,
            PData = pData,
            Creator = "流光"
        });
        try
        {
            await Task.Run(async () =>
            {
                await ItemDbService.InsertItemAsync(connection, PosOffset, BagMaxSize, itemLog);
            });
            InsertNewItem(itemLog);
            ShowMessage("发放成功", $"发放装备成功,pos={itemLog.Pos}");
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
            ShowMessage("修改成功", "修改装备成功");
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

    /// <summary>
    /// 把新物品放入列表中
    /// </summary>
    /// <param name="itemLog"></param>
    private void InsertNewItem(ItemLogViewModel itemLog)
    {
        if (_itemList is null)
        {
            return;
        }
        var insertOk = false;
        for (var i = 0; i < _itemList.Count; i++)
        {
            if (itemLog.Pos < _itemList[i].Pos)
            {
                _itemList.Insert(i, itemLog);
                insertOk = true;
                break;
            }
        }
        if (!insertOk)
        {
            _itemList.Add(itemLog);
        }
    }
}

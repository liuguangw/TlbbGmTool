using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.Models;
using liuguang.TlbbGmTool.Services;
using liuguang.TlbbGmTool.ViewModels.Data;
using liuguang.TlbbGmTool.Views.Item;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace liuguang.TlbbGmTool.ViewModels;

public class ItemListViewModel : ViewModelBase
{
    #region Fields
    public int CharGuid;
    /// <summary>
    /// 数据库连接
    /// </summary>
    public DbConnection? Connection;
    /// <summary>
    /// </summary>
    private BagType _roleBagType = BagType.ItemBag;
    /// <summary>
    /// 背包开始位置
    /// </summary>
    public int PosOffset = -1;
    /// <summary>
    /// 当前页背包最大容量
    /// </summary>
    public int BagMaxSize = 30;
    #endregion

    #region Properties

    public ObservableCollection<ItemLogViewModel> ItemList { get; } = new();
    public BagType RoleBagType
    {
        get => _roleBagType;
        set
        {
            _roleBagType = value;
            if (PosOffset < 0)
            {
                switch (_roleBagType)
                {
                    case BagType.ItemBag:
                        PosOffset = 0;
                        break;
                    case BagType.MaterialBag:
                        PosOffset = BagMaxSize;
                        break;
                    case BagType.TaskBag:
                        PosOffset = 2 * BagMaxSize;
                        break;
                }
            }
            RaisePropertyChanged(nameof(AddEquipVisible));
            RaisePropertyChanged(nameof(AddGemVisible));
        }
    }
    public Visibility AddEquipVisible => _roleBagType == BagType.ItemBag ? Visibility.Visible : Visibility.Collapsed;
    public Visibility AddGemVisible => _roleBagType == BagType.MaterialBag ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>
    /// 弹出物品编辑窗体
    /// </summary>
    public Command EditItemCommand { get; }
    /// <summary>
    /// 复制物品命令
    /// </summary>
    public Command CopyItemCommand { get; }
    /// <summary>
    /// 删除物品命令
    /// </summary>
    public Command DeleteItemCommand { get; }
    /// <summary>
    /// 显示发放装备窗体
    /// </summary>
    public Command AddEquipCommand { get; }
    /// <summary>
    /// 显示发放宝石窗体
    /// </summary>
    public Command AddGemCommand { get; }
    /// <summary>
    /// 显示发放道具窗体
    /// </summary>
    public Command AddItemCommand { get; }
    #endregion

    public ItemListViewModel()
    {
        EditItemCommand = new(ShowItemEditor);
        CopyItemCommand = new(ProcessCopyItem);
        DeleteItemCommand = new(ProcessDeleteItem);
        AddEquipCommand = new(ShowAddEquipEditor);
        AddGemCommand = new(ShowAddGemEditor);
        AddItemCommand = new(ShowAddItemEditor);
    }

    public async Task LoadItemListAsync()
    {
        if (Connection is null)
        {
            return;
        }
        try
        {
            var itemList = await Task.Run(async () =>
            {
                return await ItemDbService.LoadItemListAsync(Connection, CharGuid, PosOffset, BagMaxSize);
            });
            ItemList.Clear();
            foreach (var itemInfo in itemList)
            {
                ItemList.Add(itemInfo);
            }
        }
        catch (Exception ex)
        {
            ShowErrorMessage("加载出错", ex);
        }
    }

    private void ShowItemEditor(object? parameter)
    {
        if (parameter is not ItemLogViewModel itemLog)
        {
            return;
        }
        if (itemLog.ItemClass == 1)
        {
            ShowDialog(new EquipEditorWindow(), (EquipEditorViewModel vm) =>
            {
                vm.ItemLog = itemLog;
                vm.Connection = Connection;
            });
        }
        else if ((itemLog.ItemClass >= 2) && (itemLog.ItemClass <= 4))
        {
            ShowDialog(new CommonItemEditorWindow(), (CommonItemEditorViewModel vm) =>
            {
                vm.ItemLog = itemLog;
                vm.RoleBagType = _roleBagType;
                vm.Connection = Connection;
            });
        }
        else if (itemLog.ItemClass == 5)
        {
            ShowDialog(new GemEditorWindow(), (GemEditorViewModel vm) =>
            {
                vm.ItemLog = itemLog;
                vm.Connection = Connection;
            });
        }
        else
        {
            ShowErrorMessage("出错了", $"未知类型 class={itemLog.ItemClass}");
        }
    }
    private void ProcessCopyItem(object? parameter)
    {

    }

    private async void ProcessDeleteItem(object? parameter)
    {
        if (Connection is null)
        {
            return;
        }
        if (parameter is not ItemLogViewModel itemLog)
        {
            return;
        }
        if (!Confirm("删除提示", $"你确定要删除{itemLog.ItemName}吗?"))
        {
            return;
        }
        try
        {
            await Task.Run(async () =>
            {
                await ItemDbService.DeleteItemAsync(Connection, itemLog.Id);
            });
            ItemList.Remove(itemLog);
            ShowMessage("删除成功", $"删除{itemLog.ItemName}成功");
        }
        catch (Exception ex)
        {
            ShowErrorMessage("删除失败", ex, true);
        }

    }
    private void ShowAddEquipEditor()
    {
        ShowDialog(new EquipEditorWindow(), (EquipEditorViewModel vm) =>
        {
            vm.ItemList = ItemList;
            vm.CharGuid = CharGuid;
            vm.PosOffset = PosOffset;
            vm.BagMaxSize = BagMaxSize;
            vm.Connection = Connection;
        });
    }
    private void ShowAddGemEditor()
    {

    }
    private void ShowAddItemEditor()
    {

    }
}

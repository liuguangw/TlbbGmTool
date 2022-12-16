using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.Services;
using liuguang.TlbbGmTool.ViewModels.Data;
using liuguang.TlbbGmTool.Views.Item;
using MySql.Data.MySqlClient;

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
    /// 类型 0道具 1材料 2任务
    /// </summary>
    private int _bagType = 0;
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
    public int BagType
    {
        get => _bagType;
        set
        {
            _bagType = value;
            if (PosOffset < 0)
            {
                PosOffset = _bagType * BagMaxSize;
            }
            RaisePropertyChanged(nameof(AddEquipVisible));
            RaisePropertyChanged(nameof(AddGemVisible));
        }
    }
    public Visibility AddEquipVisible => _bagType == 0 ? Visibility.Visible : Visibility.Collapsed;
    public Visibility AddGemVisible => _bagType == 1 ? Visibility.Visible : Visibility.Collapsed;

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
                return await DoLoadItemListAsync(Connection, CharGuid);
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

    private async Task<List<ItemLogViewModel>> DoLoadItemListAsync(DbConnection connection, int charGuid)
    {
        var itemList = new List<ItemLogViewModel>();
        const string sql = "SELECT * FROM t_iteminfo WHERE charguid=@charguid AND isvalid=1 AND pos>=@pos1 AND pos<@pos2 ORDER BY pos ASC";
        var mySqlCommand = new MySqlCommand(sql, connection.Conn);
        mySqlCommand.Parameters.Add(new MySqlParameter("@charguid", MySqlDbType.Int32)
        {
            Value = charGuid
        });
        mySqlCommand.Parameters.Add(new MySqlParameter("@pos1", MySqlDbType.Int32)
        {
            Value = PosOffset
        });
        mySqlCommand.Parameters.Add(new MySqlParameter("@pos2", MySqlDbType.Int32)
        {
            Value = PosOffset + BagMaxSize
        });
        // 切换数据库
        await connection.SwitchGameDbAsync();
        using var reader = await mySqlCommand.ExecuteReaderAsync();
        if (reader is MySqlDataReader rd)
        {
            while (await rd.ReadAsync())
            {
                var pArray = new int[17];
                for (var i = 0; i < pArray.Length; i++)
                {
                    pArray[i] = rd.GetInt32("p" + (i + 1));
                }
                var pData = DataService.ConvertToPData(pArray);
                itemList.Add(new(new()
                {
                    Id = rd.GetInt32("aid"),
                    CharGuid = rd.GetInt32("charguid"),
                    Guid = rd.GetInt32("guid"),
                    World = rd.GetInt32("world"),
                    Server = rd.GetInt32("server"),
                    ItemBaseId = rd.GetInt32("itemtype"),
                    Pos = rd.GetInt32("pos"),
                    PData = pData,
                    Creator = DbStringService.ToCommonString(rd.GetString("creator")),
                    IsValid = (rd.GetInt32("isvalid") == 1),
                    DbVersion = rd.GetInt32("dbversion"),
                    FixAttr = rd.GetString("fixattr"),
                    TVar = rd.GetString("var"),
                    VisualId = rd.GetInt32("visualid"),
                    MaxgemId = rd.GetInt32("maxgemid")
                }));
            }
        }
        return itemList;
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
                vm.BagType = _bagType;
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
    private void ProcessDeleteItem(object? parameter)
    {

    }
    private void ShowAddEquipEditor()
    {

    }
    private void ShowAddGemEditor()
    {

    }
    private void ShowAddItemEditor()
    {

    }
}

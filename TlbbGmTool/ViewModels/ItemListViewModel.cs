using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.Services;
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
    /// 背包开始位置
    /// </summary>
    public int PosOffset = 0;
    /// <summary>
    /// 当前页背包最大容量
    /// </summary>
    public int BagMaxSize = 30;

    #endregion

    #region Properties

    public ObservableCollection<ItemLogViewModel> ItemList { get; } = new();

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
        using (var rd = await mySqlCommand.ExecuteReaderAsync() as MySqlDataReader)
        {
            if (rd != null)
            {
                while (await rd.ReadAsync())
                {
                    var pData = new byte[17 * 4];
                    int pValue;
                    byte[] tmpBytes;
                    for (var i = 0; i < 17; i++)
                    {
                        pValue = rd.GetInt32("p" + (i + 1));
                        tmpBytes = BitConverter.GetBytes(pValue);
                        //按小端解析
                        if (!BitConverter.IsLittleEndian)
                        {
                            Array.Reverse(tmpBytes);
                        }
                        Array.Copy(tmpBytes, 0, pData, i * 4, 4);
                    }
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
        }
        return itemList;
    }

    private void ShowItemEditor(object? parameter)
    {
        var itemLog = parameter as ItemLogViewModel;
        if(itemLog is null)
        {
            return;
        }
        if(itemLog.ItemClass == 1)
        {
            ShowDialog(new EquipEditorWindow(), (EquipEditorViewModel vm) =>
            {
                vm.ItemLog = itemLog;
                vm.Connection = Connection;
            });
        }
        else
        {
            ShowErrorMessage("todo", "todo");
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
    private void ShowAddItemEditor()
    {

    }
}

using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.Services;
using liuguang.TlbbGmTool.ViewModels.Data;
using liuguang.TlbbGmTool.Views.Item;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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
                return "发放物品";
            }
            return "修改物品 " + _itemData.EquipName;
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
    public CommonItemDataViewModel ItemData => _itemData;
    #endregion

    #region Commands
    public Command SelectItemCommand { get; }
    public Command SaveCommand { get; }
    #endregion

    public CommonItemEditorViewModel()
    {
        SelectItemCommand = new(ShowSelectItemWindow);
        SaveCommand = new(SaveItem, () => !_isSaving);
        _itemData.PropertyChanged += EquipData_PropertyChanged;
    }

    private void EquipData_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_itemData.EquipName))
        {
            RaisePropertyChanged(nameof(WindowTitle));
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
            vm.ItemList = (from itemBaseInfo in SharedData.ItemBaseMap.Values
                           where itemBaseInfo.TClass == 2 || itemBaseInfo.TClass == 3 || itemBaseInfo.TClass == 4
                           select new ItemBaseViewModel(itemBaseInfo)).ToList();
        };
        if (ShowDialog(selectorWindow, beforeAction) == true)
        {
            var selectedItem = selectorWindow.SelectedItem;
            if (selectedItem != null)
            {
                _itemData.ItemBaseId = selectedItem.ItemBaseId;
                var itemMaxSize = (byte)selectedItem.ItemMaxSize;
                _itemData.Count = Math.Min(_itemData.Count, itemMaxSize);
                _itemData.MaxSize = itemMaxSize;
                //todo fix其他字段值
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
        var itemBaseId = _itemData.ItemBaseId;
        byte[] pData = new byte[17 * 4];
        CommonItemDataService.Write(_itemData, pData);
        var pArray = DataService.ConvertToPArray(pData);
        if (_inputItemLog is null)
        {
            ShowErrorMessage("todo", "todo");
        }
        else
        {
            try
            {
                await Task.Run(async () =>
                {
                    await UpdateItemDataAsync(Connection, _inputItemLog.Id, itemBaseId, pArray);
                });
                _inputItemLog.ItemBaseId = itemBaseId;
                _inputItemLog.PData = pData;
                ShowMessage("修改成功", "修改物品成功");
                OwnedWindow?.Close();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("修改失败", ex);
            }
        }
    }

    private async Task UpdateItemDataAsync(DbConnection dbConnection, int logId, int itemBaseId, int[] pArray)
    {
        var sql = "UPDATE t_iteminfo SET itemtype=@itemtype";
        var intDictionary = new Dictionary<string, int>()
        {
            ["@itemtype"] = itemBaseId,
            ["@aid"] = logId,
        };
        for (var i = 0; i < pArray.Length; i++)
        {
            intDictionary[$"@p{i + 1}"] = pArray[i];
            sql += $",p{i + 1}=@p{i + 1}";
        }
        sql += " WHERE aid=@aid";
        var mySqlCommand = new MySqlCommand(sql, dbConnection.Conn);
        foreach (var keyPair in intDictionary)
        {
            mySqlCommand.Parameters.Add(new MySqlParameter(keyPair.Key, MySqlDbType.Int32)
            {
                Value = keyPair.Value
            });
        }
        await mySqlCommand.ExecuteNonQueryAsync();
    }
}

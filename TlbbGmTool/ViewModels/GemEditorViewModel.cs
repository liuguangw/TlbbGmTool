using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.Models;
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
public class GemEditorViewModel : ViewModelBase
{
    #region Fields
    private bool _isSaving = false;
    private ItemLogViewModel? _inputItemLog;
    private GemDataViewModel _itemData = new();
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
                _itemData.ItemBaseId = selectedItem.ItemBaseId;
                _itemData.RulerId = selectedItem.RulerId;
                if (selectedItem.BaseInfo is ItemBaseGem gemBaseInfo)
                {
                    _itemData.BasePrice = gemBaseInfo.BasePrice;
                    _itemData.AttrType = gemBaseInfo.AttrType;
                    _itemData.AttrValue = gemBaseInfo.AttrValue;
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
        var itemBaseId = _itemData.ItemBaseId;
        byte[] pData = new byte[17 * 4];
        GemDataService.Write(_itemData, pData);
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
                ShowMessage("修改成功", "修改宝石成功");
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

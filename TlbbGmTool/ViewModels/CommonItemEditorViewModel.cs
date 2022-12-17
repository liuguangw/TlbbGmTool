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
                _itemData.ItemBaseId = selectedItem.ItemBaseId;
                var itemMaxSize = selectedItem.ItemMaxSize;
                _itemData.Count = Math.Min(_itemData.Count, itemMaxSize);
                _itemData.MaxSize = itemMaxSize;
                if (selectedItem.BaseInfo is ItemBaseCommonItem itemBaseInfo)
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

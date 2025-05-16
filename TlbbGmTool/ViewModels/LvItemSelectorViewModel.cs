using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.ViewModels.Data;
using liuguang.TlbbGmTool.Views.Item;
using System;
using System.Collections.Generic;
using System.Linq;

namespace liuguang.TlbbGmTool.ViewModels;

/// <summary>
/// 宝石选择器
/// </summary>
public class LvItemSelectorViewModel : ViewModelBase
{
    #region StaticFields
    private static int LastSelectedType = 0;
    private static byte LastSelectedLevel = 0;
    private static string LastSearchText = string.Empty;
    private static int LastPage = 1;
    public static void ResetLastData()
    {
        LastSelectedType = 0;
        LastSelectedLevel = 0;
        LastSearchText = string.Empty;
        LastPage = 1;
    }
    #endregion
    #region Fields
    /// <summary>
    /// 所有的装备列表
    /// </summary>
    private List<ItemBaseViewModel> _itemList = new();
    /// <summary>
    /// 符合筛选条件的装备列表
    /// </summary>
    private List<ItemBaseViewModel> _filterItemList = new();
    private int _initItemId;

    private string _windowTitle = "物品选择器";
    private int _selectedType = 0;
    private byte _selectedLevel;
    private string _searchText = string.Empty;
    private readonly PaginationViewModel _pagination = new();
    /// <summary>
    /// 每页最大展示量
    /// </summary>
    private const int _pageLimit = 20;
    #endregion
    #region Properties
    public string WindowTitle
    {
        get => _windowTitle;
        set => SetProperty(ref _windowTitle, value);
    }
    public List<ItemBaseViewModel> ItemList
    {
        set
        {
            _itemList = value;
            LoadShortTypeSelection();
            LoadLevelSelection();
            DoFilterItemList();
        }
    }
    public PaginationViewModel Pagination => _pagination;
    public int InitItemId
    {
        set => _initItemId = value;
    }
    public int SelectedType
    {
        get => _selectedType;
        set
        {
            if (SetProperty(ref _selectedType, value))
            {
                DoFilterItemList();
            }
            LastSelectedType = value;
        }
    }
    public byte SelectedLevel
    {
        get => _selectedLevel;
        set
        {
            if (SetProperty(ref _selectedLevel, value))
            {
                DoFilterItemList();
            }
            LastSelectedLevel = value;
        }
    }
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                DoFilterItemList();
            }
            LastSearchText = value;
        }
    }

    public List<ComboBoxNode<int>> ShortTypeSelection { get; } = new() {
        new("全部",0)
    };
    public List<ComboBoxNode<byte>> LevelSelection { get; } = new() {
        new("全部",0)
    };

    public IEnumerable<ItemBaseViewModel> CurrentPageItemList
    {
        get
        {
            var offset = (_pagination.Page - 1) * _pageLimit;
            return (from itemInfo in _filterItemList
                    select itemInfo).Skip(offset).Take(_pageLimit);
        }
    }
    #endregion
    #region Commands
    public Command ConfirmCommand { get; }

    #endregion

    public LvItemSelectorViewModel()
    {
        ConfirmCommand = new(ConfirmSelect, CanConfirmSelect);
        _pagination.OnPageChanged += () =>
        {
            RaisePropertyChanged(nameof(CurrentPageItemList));
            LastPage = _pagination.Page;
        };
    }
    private void LoadShortTypeSelection()
    {
        var shortTypeNames = new List<string>();
        _itemList.ForEach(itemBaseInfo =>
        {
            if (!shortTypeNames.Contains(itemBaseInfo.ItemShortTypeString))
            {
                shortTypeNames.Add(itemBaseInfo.ItemShortTypeString);
            }
        });
        for (var i = 0; i < shortTypeNames.Count; i++)
        {
            ShortTypeSelection.Add(new(shortTypeNames[i], i + 1));
        }
        RaisePropertyChanged(nameof(ShortTypeSelection));
    }
    private void LoadLevelSelection()
    {
        var levels = new List<byte>();
        _itemList.ForEach(itemBaseInfo =>
        {
            if (!levels.Contains(itemBaseInfo.ItemLevel))
            {
                levels.Add(itemBaseInfo.ItemLevel);
            }
        });
        foreach (var levelValue in levels)
        {
            LevelSelection.Add(new($"{levelValue}级", levelValue));
        }
        RaisePropertyChanged(nameof(LevelSelection));
    }

    private void DoFilterItemList()
    {
        _filterItemList = (from itemBaseInfo in _itemList
                           where _selectedLevel == 0 || itemBaseInfo.ItemLevel == _selectedLevel
                           where _selectedType == 0 || itemBaseInfo.ItemShortTypeString == ShortTypeSelection[_selectedType].Title
                           where itemBaseInfo.ItemName.IndexOf(_searchText, StringComparison.Ordinal) >= 0
                           select itemBaseInfo).ToList();
        _pagination.SetCount(_filterItemList.Count, _pageLimit);
        RaisePropertyChanged(nameof(CurrentPageItemList));
    }
    private bool CanConfirmSelect(object? parameter)
    {
        if (parameter is ItemBaseViewModel itemBaseInfo)
        {
            return itemBaseInfo.ItemBaseId != _initItemId;

        }
        return false;
    }

    private void ConfirmSelect(object? parameter)
    {
        if (parameter is not ItemBaseViewModel itemBaseInfo)
        {
            return;
        }
        if (OwnedWindow is not LvItemSelectorWindow currentWindow)
        {
            return;
        }
        currentWindow.SelectedItem = itemBaseInfo;
        currentWindow.DialogResult = true;
        currentWindow.Close();
    }
    /// <summary>
    /// 加载上次的选项
    /// </summary>
    public void LoadLastData()
    {
        SelectedType = LastSelectedType;
        SelectedLevel = LastSelectedLevel;
        SearchText = LastSearchText;
        _pagination.Page = LastPage;
    }
}

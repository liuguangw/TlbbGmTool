using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.ViewModels.Data;
using liuguang.TlbbGmTool.Views.Item;
using System;
using System.Collections.Generic;
using System.Linq;

namespace liuguang.TlbbGmTool.ViewModels;

/// <summary>
/// 装备选择器
/// </summary>
public class ItemSelectorViewModel : ViewModelBase
{
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
    private int _minLevel;
    private string _searchText = string.Empty;

    /// <summary>
    /// 当前页码
    /// </summary>
    private int _page = 1;

    /// <summary>
    /// 总页数
    /// </summary>
    private int _pageTotal = 1;

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
            DoFilterItemList();
        }
    }
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
        }
    }
    public int MinLevel
    {
        get => _minLevel;
        set
        {
            if (SetProperty(ref _minLevel, value))
            {
                DoFilterItemList();
            }
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
        }
    }

    public List<ComboBoxNode<int>> ShortTypeSelection { get; } = new() {
        new("全部",0)
    };

    public int Page
    {
        set
        {
            if (SetProperty(ref _page, value))
            {
                RaisePropertyChanged(nameof(PageTip));
                RaisePageCommandChange();
            }
        }
    }

    private int PageTotal
    {
        set
        {
            if (SetProperty(ref _pageTotal, value))
            {
                RaisePropertyChanged(nameof(PageTip));
                RaisePageCommandChange();
            }
        }
    }

    public string PageTip => $"第{_page}/{_pageTotal}页";

    public IEnumerable<ItemBaseViewModel> CurrentPageItemList
    {
        get
        {
            var offset = (_page - 1) * _pageLimit;
            return (from itemInfo in _filterItemList
                    select itemInfo).Skip(offset).Take(_pageLimit);
        }
    }
    #endregion
    #region Commands

    public Command ConfirmCommand { get; }

    public Command FirstPageCommand { get; }
    public Command LastPageCommand { get; }
    public Command PrevPageCommand { get; }
    public Command NextPageCommand { get; }

    #endregion

    public ItemSelectorViewModel()
    {
        ConfirmCommand = new(ConfirmSelect, CanConfirmSelect);
        FirstPageCommand = new(GoToFirstPage, CanGotoFirstPage);
        LastPageCommand = new(GoToLastPage, CanGotoLastPage);
        PrevPageCommand = new(GotoPrevPage, CanGotoPrevPage);
        NextPageCommand = new(GotoNextPage, CanGotoNextPage);
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

    private void DoFilterItemList()
    {
        _filterItemList = (from itemBaseInfo in _itemList
                           where itemBaseInfo.ItemLevel >= _minLevel
                           where _selectedType == 0 || itemBaseInfo.ItemShortTypeString == ShortTypeSelection[_selectedType].Title
                           where itemBaseInfo.ItemName.IndexOf(_searchText, StringComparison.Ordinal) >= 0
                           select itemBaseInfo).ToList();
        Page = 1;
        var pageTotal = (int)Math.Ceiling(_filterItemList.Count / (double)_pageLimit);
        if (pageTotal < 1)
        {
            pageTotal = 1;
        }

        PageTotal = pageTotal;
        RaisePropertyChanged(nameof(CurrentPageItemList));
    }
    private bool CanConfirmSelect(object? parameter)
    {
        var itemBaseInfo = parameter as ItemBaseViewModel;
        if (itemBaseInfo is null)
        {
            return false;
        }
        return itemBaseInfo.ItemBaseId != _initItemId;
    }

    private void ConfirmSelect(object? parameter)
    {
        var itemBaseInfo = parameter as ItemBaseViewModel;
        if (itemBaseInfo is null)
        {
            return;
        }
        var currentWindow = OwnedWindow as ItemSelectorWindow;
        if (currentWindow is null)
        {
            return;
        }
        currentWindow.SelectedItem = itemBaseInfo;
        currentWindow.DialogResult = true;
        currentWindow.Close();
    }

    private void RaisePageCommandChange()
    {
        FirstPageCommand.RaiseCanExecuteChanged();
        LastPageCommand.RaiseCanExecuteChanged();
        PrevPageCommand.RaiseCanExecuteChanged();
        NextPageCommand.RaiseCanExecuteChanged();
    }

    private bool CanGotoFirstPage() => _page != 1;

    private void GoToFirstPage()
    {
        Page = 1;
        RaisePropertyChanged(nameof(CurrentPageItemList));
    }


    private bool CanGotoLastPage() => _page != _pageTotal;

    private void GoToLastPage()
    {
        Page = _pageTotal;
        RaisePropertyChanged(nameof(CurrentPageItemList));
    }

    private bool CanGotoPrevPage() => _page > 1;

    private void GotoPrevPage()
    {
        Page = _page - 1;
        RaisePropertyChanged(nameof(CurrentPageItemList));
    }

    private bool CanGotoNextPage() => _page < _pageTotal;

    private void GotoNextPage()
    {
        Page = _page + 1;
        RaisePropertyChanged(nameof(CurrentPageItemList));
    }
}

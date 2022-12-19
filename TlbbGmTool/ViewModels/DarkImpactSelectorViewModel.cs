using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.ViewModels.Data;
using liuguang.TlbbGmTool.Views.Item;
using System;
using System.Collections.Generic;
using System.Linq;

namespace liuguang.TlbbGmTool.ViewModels;

/// <summary>
/// 暗器技能选择器
/// </summary>
public class DarkImpactSelectorViewModel : ViewModelBase
{
    #region Fields
    /// <summary>
    /// 所有的技能列表
    /// </summary>
    private List<ComboBoxNode<int>> _itemList = new();
    /// <summary>
    /// 符合筛选条件的技能列表
    /// </summary>
    private List<ComboBoxNode<int>> _filterItemList = new();
    private int _initItemId;
    private string _searchText = string.Empty;
    private readonly PaginationViewModel _pagination = new();

    /// <summary>
    /// 每页最大展示量
    /// </summary>
    private const int _pageLimit = 20;
    #endregion
    #region Properties
    public List<ComboBoxNode<int>> ItemList
    {
        set
        {
            _itemList = value;
            DoFilterItemList();
        }
    }
    public PaginationViewModel Pagination => _pagination;
    public int InitItemId
    {
        set => _initItemId = value;
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

    public IEnumerable<ComboBoxNode<int>> CurrentPageItemList
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

    public DarkImpactSelectorViewModel()
    {
        ConfirmCommand = new(ConfirmSelect, CanConfirmSelect);
        _pagination.OnPageChanged += () =>
        {
            RaisePropertyChanged(nameof(CurrentPageItemList));
        };
    }

    private void DoFilterItemList()
    {
        _filterItemList = (from itemInfo in _itemList
                           where itemInfo.Title.IndexOf(_searchText, StringComparison.Ordinal) >= 0
                           select itemInfo).ToList();
        _pagination.SetCount(_filterItemList.Count, _pageLimit);
        RaisePropertyChanged(nameof(CurrentPageItemList));
    }
    private bool CanConfirmSelect(object? parameter)
    {
        if (parameter is ComboBoxNode<int> itemInfo)
        {
            return itemInfo.Value != _initItemId;

        }
        return false;
    }

    private void ConfirmSelect(object? parameter)
    {
        if (parameter is not ComboBoxNode<int> itemInfo)
        {
            return;
        }
        if (OwnedWindow is not DarkImpactSelectorWindow currentWindow)
        {
            return;
        }
        currentWindow.SelectedItem = itemInfo;
        currentWindow.DialogResult = true;
        currentWindow.Close();
    }
}

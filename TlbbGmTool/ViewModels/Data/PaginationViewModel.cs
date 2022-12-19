using liuguang.TlbbGmTool.Common;
using System;

namespace liuguang.TlbbGmTool.ViewModels.Data;

/// <summary>
/// 分页工具
/// </summary>
public class PaginationViewModel : NotifyBase
{
    public delegate void PageChangeHandler();
    #region Fields
    /// <summary>
    /// 当前页码
    /// </summary>
    private int _page = 1;

    /// <summary>
    /// 总页数
    /// </summary>
    private int _pageTotal = 1;

    public PageChangeHandler? OnPageChanged;
    #endregion

    #region Properties
    public int Page
    {
        get => _page;
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
    #endregion

    #region Commands
    public Command FirstPageCommand { get; }
    public Command LastPageCommand { get; }
    public Command PrevPageCommand { get; }
    public Command NextPageCommand { get; }
    #endregion
    public PaginationViewModel()
    {

        FirstPageCommand = new(GoToFirstPage, () => _page != 1);
        LastPageCommand = new(GoToLastPage, () => _page != _pageTotal);
        PrevPageCommand = new(GotoPrevPage, () => _page > 1);
        NextPageCommand = new(GotoNextPage, () => _page < _pageTotal);
    }

    /// <summary>
    /// 更新分页按钮的enable状态
    /// </summary>
    private void RaisePageCommandChange()
    {
        FirstPageCommand.RaiseCanExecuteChanged();
        LastPageCommand.RaiseCanExecuteChanged();
        PrevPageCommand.RaiseCanExecuteChanged();
        NextPageCommand.RaiseCanExecuteChanged();
    }

    /// <summary>
    /// 设置数量
    /// </summary>
    /// <param name="count">总条数</param>
    /// <param name="perCount">每页最多显示条数</param>
    public void SetCount(int count, int perCount)
    {
        Page = 1;
        if (count <= 0)
        {
            PageTotal = 1;
        }
        var pageTotal = count / perCount;
        if (count % perCount != 0)
        {
            pageTotal++;
        }
        PageTotal = pageTotal;
    }

    private void GoToFirstPage()
    {
        Page = 1;
        OnPageChanged?.Invoke();
    }

    private void GoToLastPage()
    {
        Page = _pageTotal;
        OnPageChanged?.Invoke();
    }

    private void GotoPrevPage()
    {
        Page = _page - 1;
        OnPageChanged?.Invoke();
    }

    private void GotoNextPage()
    {
        Page = _page + 1;
        OnPageChanged?.Invoke();
    }
}

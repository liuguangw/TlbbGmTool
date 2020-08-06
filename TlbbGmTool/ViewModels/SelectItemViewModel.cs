using System;
using System.Collections.Generic;
using System.Linq;
using TlbbGmTool.Core;
using TlbbGmTool.Models;
using TlbbGmTool.View.Windows;

namespace TlbbGmTool.ViewModels
{
    public class SelectItemViewModel : BindDataBase
    {
        #region Fields

        /// <summary>
        /// item库
        /// </summary>
        private List<ItemBase> _itemBaseList;

        /// <summary>
        /// 符合筛选条件的所有item的列表
        /// </summary>
        private List<ItemBase> _filterItemList = new List<ItemBase>();

        /// <summary>
        /// 当前选择窗口
        /// </summary>
        private SelectItemWindow _selectWindow;

        /// <summary>
        /// 初始时的item id
        /// </summary>
        private int _initItemId;

        private int _shortType;
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

        public List<ComboBoxNode<int>> ShortTypeSelection { get; private set; }
            = new List<ComboBoxNode<int>>();

        public int ShortType
        {
            get => _shortType;
            set
            {
                if (SetProperty(ref _shortType, value))
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

        public IEnumerable<ItemBase> CurrentPageItemList
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

        public AppCommand ConfirmCommand { get; }

        public AppCommand FirstPageCommand { get; }
        public AppCommand LastPageCommand { get; }
        public AppCommand PrevPageCommand { get; }
        public AppCommand NextPageCommand { get; }

        #endregion

        public SelectItemViewModel()
        {
            ConfirmCommand = new AppCommand(ConfirmSelect, CanConfirmSelect);
            FirstPageCommand = new AppCommand(GoToFirstPage, CanGotoFirstPage);
            LastPageCommand = new AppCommand(GoToLastPage, CanGotoLastPage);
            PrevPageCommand = new AppCommand(GotoPrevPage, CanGotoPrevPage);
            NextPageCommand = new AppCommand(GotoNextPage, CanGotoNextPage);
        }

        public void InitData(List<ItemBase> itemBaseList, SelectItemWindow selectWindow, int initItemId)
        {
            _itemBaseList = itemBaseList;
            _selectWindow = selectWindow;
            _initItemId = initItemId;
            LoadShortTypeSelection();
            DoFilterItemList();
        }

        private void LoadShortTypeSelection()
        {
            var shortTypeNames = new List<string> {"全部"};
            _itemBaseList.ForEach(itemBaseInfo =>
            {
                if (!shortTypeNames.Contains(itemBaseInfo.ShortTypeString))
                {
                    shortTypeNames.Add(itemBaseInfo.ShortTypeString);
                }
            });
            ShortTypeSelection =
                shortTypeNames.Select(
                    (itemName, itemIndex)
                        =>
                        new ComboBoxNode<int> {Title = itemName, Value = itemIndex}
                ).ToList();
            RaisePropertyChanged(nameof(ShortTypeSelection));
        }

        private void DoFilterItemList()
        {
            _filterItemList = (from itemBaseInfo in _itemBaseList
                where itemBaseInfo.Level >= _minLevel
                where _shortType == 0 || itemBaseInfo.ShortTypeString == ShortTypeSelection[_shortType].Title
                where itemBaseInfo.Name.IndexOf(_searchText, StringComparison.Ordinal) >= 0
                select itemBaseInfo).ToList();
            Page = 1;
            var pageTotal = (int) Math.Ceiling(_filterItemList.Count / (double) _pageLimit);
            if (pageTotal < 1)
            {
                pageTotal = 1;
            }

            PageTotal = pageTotal;
            RaisePropertyChanged(nameof(CurrentPageItemList));
        }

        private bool CanConfirmSelect(object parameter)
        {
            var itemBaseInfo = parameter as ItemBase;
            return itemBaseInfo.Id != _initItemId;
        }

        private void ConfirmSelect(object parameter)
        {
            var itemBaseInfo = parameter as ItemBase;
            _selectWindow.TargetItem = itemBaseInfo;
            _selectWindow.DialogResult = true;
            _selectWindow.Close();
        }

        #region PageMethods

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

        #endregion
    }
}
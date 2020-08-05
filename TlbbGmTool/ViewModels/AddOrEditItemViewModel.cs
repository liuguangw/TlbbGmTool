using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TlbbGmTool.Core;
using TlbbGmTool.Models;
using TlbbGmTool.View.Windows;

namespace TlbbGmTool.ViewModels
{
    public class AddOrEditItemViewModel : BindDataBase
    {
        #region Fields

        private MainWindowViewModel _mainWindowViewModel;
        private ItemInfo _itemInfo;
        private int _charguid;
        private bool _isAdd = true;
        private bool _isMaterial = true;
        private ObservableCollection<ItemInfo> _bagItemList;

        private AddOrEditItemWindow _addOrEditItemWindow;

        //
        private List<ItemBase> _itemBaseList = new List<ItemBase>();

        //
        private int _itemBaseId;
        private int _itemMaxSize = 1;
        private int _itemCurrentSize = 1;

        #endregion

        #region Properties

        public string WindowTitle
        {
            get
            {
                if (_isAdd)
                {
                    return "添加" + (_isMaterial ? "材料" : "物品");
                }

                return "修改 " + ItemName;
            }
        }

        public string ItemName
        {
            get
            {
                if (_itemBaseId == 0)
                {
                    return "无";
                }

                var itemBaseInfo = FindItemById(_itemBaseId, _itemBaseList);
                return itemBaseInfo != null
                    ? FormatItemName(itemBaseInfo)
                    : FormatItemName(null, _itemBaseId);
            }
        }

        public int ItemBaseId
        {
            set
            {
                if (SetProperty(ref _itemBaseId, value))
                {
                    RaisePropertyChanged(nameof(ItemName));
                    RaisePropertyChanged(nameof(WindowTitle));
                    RaisePropertyChanged(nameof(ItemMaxSize));
                }
            }
        }

        public int ItemMaxSize
        {
            get => _itemMaxSize;
            set => SetProperty(ref _itemMaxSize, value);
        }

        public int ItemCurrentSize
        {
            get => _itemCurrentSize;
            set => SetProperty(ref _itemCurrentSize, value);
        }

        #endregion

        public void InitData(MainWindowViewModel mainWindowViewModel, ItemInfo itemInfo,
            int charguid, bool isMaterial, ObservableCollection<ItemInfo> itemList,
            AddOrEditItemWindow addOrEditItemWindow)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _addOrEditItemWindow = addOrEditItemWindow;
            _itemBaseList = (from itemBaseInfoPair in _mainWindowViewModel.ItemBases
                where itemBaseInfoPair.Value.ItemClass == (isMaterial ? 2 : 3)
                select itemBaseInfoPair.Value).ToList();
            _charguid = charguid;
            _bagItemList = itemList;
            _isMaterial = isMaterial;
            if (itemInfo == null)
            {
                //初始化默认值
                var firstItem = _itemBaseList.First();
                ItemBaseId = firstItem.Id;
                ItemMaxSize = firstItem.MaxSize;
                ItemCurrentSize = firstItem.MaxSize;
                return;
            }

            _itemInfo = itemInfo;
            _isAdd = false;
            //初始化属性
            ItemBaseId = itemInfo.ItemType;
            ItemMaxSize = itemInfo.MaxSize;
            ItemCurrentSize = itemInfo.ItemCount;
        }

        private static ItemBase FindItemById(int itemId, IEnumerable<ItemBase> itemBaseList)
        {
            if (itemId == 0)
            {
                return null;
            }

            return (from baseInfo in itemBaseList
                where baseInfo.Id == itemId
                select baseInfo).First();
        }

        private static string FormatItemName(ItemBase itemBaseInfo, int itemId = 0)
        {
            return itemBaseInfo != null
                ? $"{itemBaseInfo.Name}(ID: {itemBaseInfo.Id})"
                : $"未知(ID: {itemId})";
        }
    }
}
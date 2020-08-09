using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TlbbGmTool.Core;
using TlbbGmTool.Models;
using TlbbGmTool.Services;
using TlbbGmTool.View.Windows;

namespace TlbbGmTool.ViewModels
{
    public class AddOrEditItemViewModel : BindDataBase
    {
        public enum ItemCategory
        {
            CommonItem,
            Material,
            Gem
        }

        #region Fields

        private MainWindowViewModel _mainWindowViewModel;
        private ItemInfo _itemInfo;
        private int _charguid;
        private bool _isAdd = true;
        private ItemCategory _itemCategory;
        private ObservableCollection<ItemInfo> _bagItemList;

        private AddOrEditItemWindow _addOrEditItemWindow;

        //
        private List<ItemBase> _itemBaseList = new List<ItemBase>();

        //
        private int _itemBaseId;
        private int _itemMaxSize = 1;
        private int _itemCount = 1;

        #endregion

        #region Properties

        public string WindowTitle
        {
            get
            {
                if (!_isAdd)
                {
                    return "修改 " + ItemName;
                }

                var targetName = string.Empty;
                switch (_itemCategory)
                {
                    case ItemCategory.CommonItem:
                        targetName = "物品";
                        break;
                    case ItemCategory.Material:
                        targetName = "材料";
                        break;
                    case ItemCategory.Gem:
                        targetName = "宝石";
                        break;
                }

                return "发放" + targetName;
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
                }
            }
        }

        public int ItemMaxSize
        {
            get => _itemMaxSize;
            set
            {
                if (SetProperty(ref _itemMaxSize, value))
                {
                    RaisePropertyChanged(nameof(CanEditCount));
                }
            }
        }

        public int ItemCount
        {
            get => _itemCount;
            set => SetProperty(ref _itemCount, value);
        }

        /// <summary>
        /// 是否可以修改个数
        /// </summary>
        public bool CanEditCount => _itemMaxSize > 1;

        #endregion

        #region Commands

        public AppCommand SelectItemCommand { get; }
        public AppCommand SaveItemCommand { get; }

        #endregion

        public AddOrEditItemViewModel()
        {
            SelectItemCommand = new AppCommand(SelectItem);
            SaveItemCommand = new AppCommand(SaveItem);
        }

        public void InitData(MainWindowViewModel mainWindowViewModel, ItemInfo itemInfo,
            int charguid, ItemCategory itemCategory, ObservableCollection<ItemInfo> itemList,
            AddOrEditItemWindow addOrEditItemWindow)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _addOrEditItemWindow = addOrEditItemWindow;
            var itemClass = 0;
            switch (itemCategory)
            {
                case ItemCategory.CommonItem:
                    itemClass = 3;
                    break;
                case ItemCategory.Material:
                    itemClass = 2;
                    break;
                case ItemCategory.Gem:
                    itemClass = 5;
                    break;
            }

            _itemBaseList = (from itemBaseInfoPair in _mainWindowViewModel.ItemBases
                where itemBaseInfoPair.Value.ItemClass == itemClass
                select itemBaseInfoPair.Value).ToList();
            _charguid = charguid;
            _bagItemList = itemList;
            _itemCategory = itemCategory;
            if (itemInfo == null)
            {
                //初始化默认值
                var firstItem = _itemBaseList.First();
                ItemBaseId = firstItem.Id;
                ItemMaxSize = firstItem.MaxSize;
                ItemCount = firstItem.MaxSize;
                return;
            }

            _itemInfo = itemInfo;
            _isAdd = false;
            //初始化属性
            ItemBaseId = itemInfo.ItemType;
            ItemMaxSize = itemInfo.MaxSize;
            ItemCount = itemInfo.ItemCount;
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

        private void SelectItem()
        {
            var selectItemWindow = new SelectItemWindow(_itemBaseList, _itemBaseId)
            {
                Owner = _addOrEditItemWindow
            };
            if (selectItemWindow.ShowDialog() == true)
            {
                var selectedItem = selectItemWindow.TargetItem;
                ItemBaseId = selectedItem.Id;
                ItemMaxSize = selectedItem.MaxSize;
                ItemCount = selectedItem.MaxSize;
            }
        }

        private async void SaveItem()
        {
            ItemInfo itemInfo;
            try
            {
                itemInfo = await DoSaveItem();
            }
            catch (Exception e)
            {
                _mainWindowViewModel.ShowErrorMessage("保存失败", e.Message);
                return;
            }

            if (_isAdd)
            {
                var bagType = SaveItemService.BagType.ItemBag;
                if (_itemCategory == ItemCategory.Gem || _itemCategory == ItemCategory.Material)
                {
                    bagType = SaveItemService.BagType.MaterialBag;
                }

                //计算插入列表的位置
                var (startPos, _) = SaveItemService.GetBagItemIndexRange(bagType);
                var insertIndex = itemInfo.Pos - startPos;
                _bagItemList.Insert(insertIndex, itemInfo);
            }
            else
            {
                //更新属性
                _itemInfo.ItemType = itemInfo.ItemType;
                _itemInfo.PArray = itemInfo.PArray;
                _itemInfo.RaiseItemCountChange();
            }

            //更新标题
            RaisePropertyChanged(nameof(WindowTitle));
            _mainWindowViewModel.ShowSuccessMessage("保存成功", $"保存物品信息成功(pos={itemInfo.Pos})");
            _addOrEditItemWindow.Close();
        }

        private async Task<ItemInfo> DoSaveItem()
        {
            var itemType = _itemBaseId;
            var itemBases = _mainWindowViewModel.ItemBases;
            if (!itemBases.ContainsKey(itemType))
            {
                throw new Exception($"无效的物品ID: {itemType}");
            }

            //防止不正确的数量,或者数量超出堆叠上限
            var itemCount = _itemCount;
            if (itemCount < 1)
            {
                itemCount = 1;
            }
            else if (itemCount > itemBases[itemType].MaxSize)
            {
                itemCount = itemBases[itemType].MaxSize;
            }

            var pArray =
                _itemInfo == null ? new int[17] : _itemInfo.PArray;
            pArray[6] &= 0xffffff;
            pArray[6] |= itemCount << 24;

            //获取数据库数据连接
            var mySqlConnection = _mainWindowViewModel.MySqlConnection;
            var gameDbName = _mainWindowViewModel.SelectedServer.GameDbName;
            await SaveItemService.PrepareConnection(mySqlConnection, gameDbName);
            ItemInfo itemInfo;
            if (_itemInfo == null)
            {
                pArray[0] = 65540;
                pArray[1] = 65536;
                pArray[2] = 16842752;
                pArray[3] = -1;
                pArray[4] = -1;
                pArray[5] = -1;
                pArray[6] |= 0xFFFF;
                var bagType = SaveItemService.BagType.ItemBag;
                if (_itemCategory == ItemCategory.Material || _itemCategory == ItemCategory.Gem)
                {
                    bagType = SaveItemService.BagType.MaterialBag;
                }

                itemInfo = await SaveItemService.InsertItemAsync(mySqlConnection, itemType, pArray,
                    _charguid, itemBases, bagType, string.Empty);
            }
            else
            {
                itemInfo = await SaveItemService.UpdateItemAsync(mySqlConnection, itemType, pArray,
                    _charguid, itemBases, _itemInfo.Pos);
            }

            return itemInfo;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using TlbbGmTool.Core;
using TlbbGmTool.Models;
using TlbbGmTool.View.Windows;

namespace TlbbGmTool.ViewModels
{
    public class AddOrEditEquipViewModel : BindDataBase
    {
        #region Fields

        private MainWindowViewModel _mainWindowViewModel;
        private ItemInfo _itemInfo;
        private bool _isAddEquip = true;
        private AddOrEditEquipWindow _addOrEditEquipWindow;

        //
        private List<ItemBase> _equipBaseList = new List<ItemBase>();
        private List<ItemBase> _gemBaseList = new List<ItemBase>();
        private int _itemBaseId;
        private int _starCount = 0;
        private int _slotCount = 0;
        private int _enhanceCount = 0;
        private int _floatValue = 0;
        private int _attr1 = 0;

        private int _attr2 = 0;

        //
        private int _gem1;
        private int _gem2;
        private int _gem3;

        private int _gem4;

        //
        private int _qualification1;
        private int _qualification2;
        private int _qualification3;
        private int _qualification4;
        private int _qualification5;

        private int _qualification6;

        //
        private bool _bindStatus;
        private bool _verifiedStatus;
        private bool _qualificationVerifiedStatus;
        private bool _engravedStatus;
        private int _equipVisual;

        #endregion

        #region Properties

        public string WindowTitle => _isAddEquip ? "添加装备" : $"修改装备 {ItemName})";

        public AppCommand SaveEquipCommand { get; }

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

        public string ItemName => FindItemName(_itemBaseId, _equipBaseList);

        public List<ComboBoxNode<int>> StarSection { get; } = new List<ComboBoxNode<int>>();

        public int StarCount
        {
            get => _starCount;
            set => SetProperty(ref _starCount, value);
        }

        public List<ComboBoxNode<int>> SlotSelection { get; } = new List<ComboBoxNode<int>>();

        public int SlotCount
        {
            get => _slotCount;
            set => SetProperty(ref _slotCount, value);
        }

        public int EnhanceCount
        {
            get => _enhanceCount;
            set => SetProperty(ref _enhanceCount, value);
        }

        public int FloatValue
        {
            get => _floatValue;
            set => SetProperty(ref _floatValue, value);
        }

        public int Attr1
        {
            set
            {
                if (SetProperty(ref _attr1, value))
                {
                    RaisePropertyChanged(nameof(Attr1Tip));
                }
            }
        }

        public string Attr1Tip => CalcAttrTip(_attr1);

        public int Attr2
        {
            set
            {
                if (SetProperty(ref _attr2, value))
                {
                    RaisePropertyChanged(nameof(Attr2Tip));
                }
            }
        }

        public string Attr2Tip => CalcAttrTip(_attr2);

        public int Gem1
        {
            set
            {
                if (SetProperty(ref _gem1, value))
                {
                    RaisePropertyChanged(nameof(Gem1Name));
                }
            }
        }

        public int Gem2
        {
            set
            {
                if (SetProperty(ref _gem2, value))
                {
                    RaisePropertyChanged(nameof(Gem2Name));
                }
            }
        }

        public int Gem3
        {
            set
            {
                if (SetProperty(ref _gem3, value))
                {
                    RaisePropertyChanged(nameof(Gem3Name));
                }
            }
        }

        public int Gem4
        {
            set
            {
                if (SetProperty(ref _gem4, value))
                {
                    RaisePropertyChanged(nameof(Gem4Name));
                }
            }
        }

        public string Gem1Name => FindItemName(_gem1, _gemBaseList);

        public string Gem2Name => FindItemName(_gem2, _gemBaseList);

        public string Gem3Name => FindItemName(_gem3, _gemBaseList);

        public string Gem4Name => FindItemName(_gem4, _gemBaseList);

        public int Qualification1
        {
            get => _qualification1;
            set => SetProperty(ref _qualification1, value);
        }

        public int Qualification2
        {
            get => _qualification2;
            set => SetProperty(ref _qualification2, value);
        }

        public int Qualification3
        {
            get => _qualification3;
            set => SetProperty(ref _qualification3, value);
        }

        public int Qualification4
        {
            get => _qualification4;
            set => SetProperty(ref _qualification4, value);
        }

        public int Qualification5
        {
            get => _qualification5;
            set => SetProperty(ref _qualification5, value);
        }

        public int Qualification6
        {
            get => _qualification6;
            set => SetProperty(ref _qualification6, value);
        }

        public bool BindStatus
        {
            get => _bindStatus;
            set => SetProperty(ref _bindStatus, value);
        }

        public bool VerifiedStatus
        {
            get => _verifiedStatus;
            set => SetProperty(ref _verifiedStatus, value);
        }

        public bool QualificationVerifiedStatus
        {
            get => _qualificationVerifiedStatus;
            set => SetProperty(ref _qualificationVerifiedStatus, value);
        }

        public bool EngravedStatus
        {
            get => _engravedStatus;
            set => SetProperty(ref _engravedStatus, value);
        }

        public int EquipVisual
        {
            set
            {
                if (SetProperty(ref _equipVisual, value))
                {
                    RaisePropertyChanged(nameof(EquipVisualName));
                }
            }
        }

        public string EquipVisualName
        {
            get
            {
                if (_equipVisual == 0)
                {
                    return "未知";
                }

                var itemBaseInfo = (from baseInfo in _equipBaseList
                    where baseInfo.EquipVisual == _equipVisual
                    select baseInfo).First();
                return itemBaseInfo != null
                    ? $"{itemBaseInfo.Name}(ID: {itemBaseInfo.Id})"
                    : $"未知(Visual: {_equipVisual})";
            }
        }

        #endregion

        public AddOrEditEquipViewModel()
        {
            SaveEquipCommand = new AppCommand(SaveEquip);
            for (var i = 0; i <= 9; i++)
            {
                StarSection.Add(new ComboBoxNode<int>()
                {
                    Title = $"{i}星",
                    Value = i
                });
                if (i <= 4)
                {
                    SlotSelection.Add(new ComboBoxNode<int>()
                    {
                        Title = $"{i}孔",
                        Value = i
                    });
                }
            }
        }

        public void InitData(MainWindowViewModel mainWindowViewModel, ItemInfo itemInfo,
            AddOrEditEquipWindow addOrEditEquipWindow)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _addOrEditEquipWindow = addOrEditEquipWindow;
            _equipBaseList = (from itemBaseInfoPair in _mainWindowViewModel.ItemBases
                where itemBaseInfoPair.Value.ItemClass == 1
                select itemBaseInfoPair.Value).ToList();
            _gemBaseList = (from itemBaseInfoPair in _mainWindowViewModel.ItemBases
                where itemBaseInfoPair.Value.ItemClass == 5
                select itemBaseInfoPair.Value).ToList();
            if (itemInfo == null)
            {
                //初始化默认值
                var firstItem = _equipBaseList.First();
                ItemBaseId = firstItem.Id;
                EquipVisual = firstItem.Id;
                return;
            }

            _itemInfo = itemInfo;
            _isAddEquip = false;
            //初始化属性
            LoadEquipInfo();
        }

        /// <summary>
        /// 从itemInfo中加载equip信息
        /// </summary>
        private void LoadEquipInfo()
        {
            ItemBaseId = _itemInfo.ItemType;
            StarCount = (_itemInfo.PArray[8] >> 8) & 0xff;
            SlotCount = _itemInfo.PArray[4] & 0xff;
            EquipVisual = (_itemInfo.PArray[8] >> 16) & 0xffff;
            Gem1 = ((_itemInfo.PArray[1] & 0xffff) << 16) + (_itemInfo.PArray[0] >> 16);
            Gem2 = ((_itemInfo.PArray[2] & 0xffff) << 16) + (_itemInfo.PArray[1] >> 16);
            Gem3 = ((_itemInfo.PArray[3] & 0xffff) << 16) + (_itemInfo.PArray[2] >> 16);
            Gem4 = ((_itemInfo.PArray[16] & 0xff) << 24) + ((_itemInfo.PArray[15] >> 8) & 0xffffff);
            Attr1 = _itemInfo.PArray[9];
            Attr2 = _itemInfo.PArray[10];
            EnhanceCount = _itemInfo.PArray[5] >> 24;
            FloatValue = _itemInfo.PArray[11] & 0xff;
            Qualification1 = (_itemInfo.PArray[6] >> 24) & 0xff;
            Qualification2 = (_itemInfo.PArray[7] >> 8) & 0xff;
            Qualification3 = _itemInfo.PArray[7] & 0xff;
            Qualification4 = (_itemInfo.PArray[7] >> 16) & 0xff;
            Qualification5 = (_itemInfo.PArray[7] >> 24) & 0xff;
            Qualification6 = _itemInfo.PArray[8] & 0xff;
            var statusVal = (_itemInfo.PArray[3] >> 16) & 0xff;
            BindStatus = (statusVal & 1) != 0;
            VerifiedStatus = ((statusVal >> 1) & 1) != 0;
            QualificationVerifiedStatus = ((statusVal >> 5) & 1) != 0;
            EngravedStatus = ((statusVal >> 6) & 1) != 0;
        }

        private async void SaveEquip()
        {
        }

        /// <summary>
        /// 通过ID查找名称
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="itemBaseList"></param>
        /// <returns></returns>
        private static string FindItemName(int itemId, IEnumerable<ItemBase> itemBaseList)
        {
            if (itemId == 0)
            {
                return "无";
            }

            var itemBaseInfo = (from baseInfo in itemBaseList
                where baseInfo.Id == itemId
                select baseInfo).First();
            return itemBaseInfo != null ? $"{itemBaseInfo.Name}(ID: {itemId})" : $"未知(ID: {itemId})";
        }

        private static string CalcAttrTip(int attrValue)
        {
            var attrCount = 0;
            for (var i = 0; i < 32; i++)
            {
                var tmpValue = attrValue;
                if (i > 0)
                {
                    tmpValue >>= i;
                }

                if ((tmpValue & 1) != 0)
                {
                    attrCount++;
                }
            }

            return $"已选择{attrCount}种属性";
        }
    }
}
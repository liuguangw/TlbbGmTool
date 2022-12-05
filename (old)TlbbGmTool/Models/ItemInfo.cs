using System.Collections.Generic;
using TlbbGmTool.Core;

namespace TlbbGmTool.Models
{
    public class ItemInfo : BindDataBase
    {
        #region Fields

        private int _charguid;
        private int _guid;
        private int _world;
        private int _server;
        private int _itemType;
        private int _pos;
        private int[] _pArray;
        private string _creator;

        private readonly Dictionary<int, ItemBase> _itemBases;

        private ItemBase _currentItemBase;

        #endregion

        #region Properties

        public int Charguid
        {
            get => _charguid;
            set => SetProperty(ref _charguid, value);
        }

        public int Guid
        {
            get => _guid;
            set => SetProperty(ref _guid, value);
        }

        public int World
        {
            get => _world;
            set => SetProperty(ref _world, value);
        }

        public int Server
        {
            get => _server;
            set => SetProperty(ref _server, value);
        }

        public int ItemType
        {
            get => _itemType;
            set
            {
                if (SetProperty(ref _itemType, value))
                {
                    UpdateCurrentItemBase();
                }
            }
        }

        public int Pos
        {
            get => _pos;
            set => SetProperty(ref _pos, value);
        }

        public int[] PArray
        {
            get => _pArray;
            set => SetProperty(ref _pArray, value);
        }

        public string Creator
        {
            get => _creator;
            set => SetProperty(ref _creator, value);
        }

        /// <summary>
        /// 是否为未知物品
        /// </summary>
        public bool IsUnknownItem => _currentItemBase == null;

        #endregion

        #region DisplayProperties

        public string Name =>
            _currentItemBase?.Name ?? "未知物品";

        public string ShortTypeString =>
            _currentItemBase?.ShortTypeString ?? "未知";

        public string Description =>
            _currentItemBase?.Description ?? "未知";

        public int Level =>
            _currentItemBase?.Level ?? 0;

        public int MaxSize =>
            _currentItemBase?.MaxSize ?? 0;

        public int ItemBaseClass =>
            _currentItemBase?.ItemClass ?? 0;

        public int ItemBaseType =>
            _currentItemBase?.ItemType ?? 0;

        public int ItemCount
        {
            get
            {
                if (_currentItemBase == null)
                {
                    return 1;
                }

                if (_currentItemBase.MaxSize == 1)
                {
                    return 1;
                }
                //读取个数

                return (PArray[6] >> 24) & 0xff;
            }
        }

        public ItemBase CurrentItemBase => _currentItemBase;

        #endregion

        public ItemInfo(Dictionary<int, ItemBase> itemBases)
        {
            _itemBases = itemBases;
        }

        private void UpdateCurrentItemBase()
        {
            _currentItemBase = _itemBases.ContainsKey(_itemType) ? _itemBases[_itemType] : null;
            RaisePropertyChanged(nameof(Name));
            RaisePropertyChanged(nameof(ShortTypeString));
            RaisePropertyChanged(nameof(Description));
            RaisePropertyChanged(nameof(Level));
            RaisePropertyChanged(nameof(ItemCount));
            RaisePropertyChanged(nameof(MaxSize));
            RaisePropertyChanged(nameof(ItemBaseClass));
            RaisePropertyChanged(nameof(ItemBaseType));
        }

        public void RaiseItemCountChange()
        {
            RaisePropertyChanged(nameof(ItemCount));
        }
    }
}
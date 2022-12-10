using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.Models;

namespace liuguang.TlbbGmTool.ViewModels;

public class ItemLogViewModel : NotifyBase
{
    #region Fields
    private ItemLog _itemLog;
    #endregion
    #region Properties
    public int Id
    {
        get => _itemLog.Id;
        private set => SetProperty(ref _itemLog.Id, value);
    }
    public int CharGuid
    {
        get => _itemLog.CharGuid;
        set => SetProperty(ref _itemLog.CharGuid, value);
    }
    public int Guid
    {
        get => _itemLog.Guid;
        set => SetProperty(ref _itemLog.Guid, value);
    }
    public int World
    {
        get => _itemLog.World;
        private set => SetProperty(ref _itemLog.World, value);
    }
    public int Server
    {
        get => _itemLog.Server;
        private set => SetProperty(ref _itemLog.Server, value);
    }
    public int ItemBaseId
    {
        get => _itemLog.ItemBaseId;
        set
        {
            if (SetProperty(ref _itemLog.ItemBaseId, value))
            {
                RaiseBaseIdChange();
            }
        }
    }
    public int Pos
    {
        get => _itemLog.Pos;
        set => SetProperty(ref _itemLog.Pos, value);
    }
    public byte[] PData
    {
        get => _itemLog.PData;
        set
        {
            if (SetProperty(ref _itemLog.PData, value))
            {
                RaisePropertyChanged(nameof(ItemCount));
            }
        }
    }
    public string Creator
    {
        get => _itemLog.Creator;
        set => SetProperty(ref _itemLog.Creator, value);
    }
    public bool IsValid
    {
        get => _itemLog.IsValid;
        private set => SetProperty(ref _itemLog.IsValid, value);
    }
    public int DbVersion
    {
        get => _itemLog.DbVersion;
        private set => SetProperty(ref _itemLog.DbVersion, value);
    }
    public string FixAttr
    {
        get => _itemLog.FixAttr;
        private set => SetProperty(ref _itemLog.FixAttr, value);
    }
    public string TVar
    {
        get => _itemLog.TVar;
        private set => SetProperty(ref _itemLog.TVar, value);
    }
    public int VisualId
    {
        get => _itemLog.VisualId;
        set => SetProperty(ref _itemLog.VisualId, value);
    }
    public int MaxgemId
    {
        get => _itemLog.MaxgemId;
        private set => SetProperty(ref _itemLog.MaxgemId, value);
    }
    public string ItemName
    {
        get
        {
            if (SharedData.ItemBaseMap.TryGetValue(_itemLog.ItemBaseId, out var itemBaseInfo))
            {
                return itemBaseInfo.Name;
            }
            return "未知物品";
        }
    }
    public string ItemShortTypeString
    {
        get
        {
            if (SharedData.ItemBaseMap.TryGetValue(_itemLog.ItemBaseId, out var itemBaseInfo))
            {
                return itemBaseInfo.ShortTypeString;
            }
            return "未知物品";
        }
    }
    public string ItemDescription
    {
        get
        {
            if (SharedData.ItemBaseMap.TryGetValue(_itemLog.ItemBaseId, out var itemBaseInfo))
            {
                return itemBaseInfo.Description;
            }
            return "未知物品";
        }
    }
    public int ItemLevel
    {
        get
        {
            if (SharedData.ItemBaseMap.TryGetValue(_itemLog.ItemBaseId, out var itemBaseInfo))
            {
                return itemBaseInfo.Level;
            }
            return 0;
        }
    }
    public int ItemMaxSize
    {
        get
        {
            if (SharedData.ItemBaseMap.TryGetValue(_itemLog.ItemBaseId, out var itemBaseInfo))
            {
                return itemBaseInfo.MaxSize;
            }
            return 0;
        }
    }
    public int ItemClass
    {
        get
        {
            if (SharedData.ItemBaseMap.TryGetValue(_itemLog.ItemBaseId, out var itemBaseInfo))
            {
                return itemBaseInfo.TClass;
            }
            return 0;
        }
    }
    public int ItemType
    {
        get
        {
            if (SharedData.ItemBaseMap.TryGetValue(_itemLog.ItemBaseId, out var itemBaseInfo))
            {
                return itemBaseInfo.TType;
            }
            return 0;
        }
    }
    public int ItemCount
    {
        get
        {
            if (ItemMaxSize == 1)
            {
                return 1;
            }
            return _itemLog.PData[6 * 4 + 3] & 0xff;
        }
    }
    #endregion
    public ItemLogViewModel(ItemLog itemLog)
    {
        _itemLog = itemLog;
    }
    private void RaiseBaseIdChange()
    {
        RaisePropertyChanged(nameof(ItemName));
        RaisePropertyChanged(nameof(ItemShortTypeString));
        RaisePropertyChanged(nameof(ItemDescription));
        RaisePropertyChanged(nameof(ItemLevel));
        RaisePropertyChanged(nameof(ItemMaxSize));
        RaisePropertyChanged(nameof(ItemClass));
        RaisePropertyChanged(nameof(ItemType));
        RaisePropertyChanged(nameof(ItemCount));
    }
}

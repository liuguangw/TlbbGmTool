using System.Collections.Generic;
using System.Collections.ObjectModel;
using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.Models;

namespace liuguang.TlbbGmTool.ViewModels.Data;
public class BagContainer : NotifyBase
{

    #region Fields
    private ObservableCollection<ItemLogViewModel> _itemList = new();
    /// <summary>
    /// 角色id
    /// </summary>
    public int CharGuid;
    /// <summary>
    /// 背包开始位置
    /// </summary>
    public int PosOffset = -1;

    private BagType _roleBagType = BagType.ItemBag;
    /// <summary>
    /// 当前页背包最大容量
    /// </summary>
    public int BagMaxSize = 30;
    #endregion
    public ObservableCollection<ItemLogViewModel> ItemList => _itemList;

    public ServerType GameServerType = ServerType.Common;
    public BagType RoleBagType
    {
        get => _roleBagType;
        set
        {
            if (GameServerType == ServerType.Common)
            {
                BagMaxSize = 30;
            }
            else
            {
                if (value == BagType.ItemBag || value == BagType.MaterialBag)
                {
                    BagMaxSize = 100;

                }
                else
                {
                    BagMaxSize = 40;
                }
            }
            SetProperty(ref _roleBagType, value);
            if (PosOffset < 0)
            {
                switch (_roleBagType)
                {
                    case BagType.ItemBag:
                        PosOffset = 0;
                        break;
                    case BagType.MaterialBag:
                        PosOffset = BagMaxSize;
                        break;
                    case BagType.TaskBag:
                        if (GameServerType == ServerType.Common)
                        { 
                            PosOffset = 2 * BagMaxSize;
                        }
                        else
                        {
                            PosOffset = 200;
                        }
                        break;
                }
            }
        }
    }

    public void FillItemList(List<ItemLogViewModel> itemList)
    {
        _itemList.Clear();
        foreach (var itemInfo in itemList)
        {
            _itemList.Add(itemInfo);
        }
    }

    /// <summary>
    /// 把新物品放入列表中
    /// </summary>
    /// <param name="itemLog"></param>
    public void InsertNewItem(ItemLogViewModel itemLog)
    {
        var insertOk = false;
        for (var i = 0; i < _itemList.Count; i++)
        {
            if (itemLog.Pos < _itemList[i].Pos)
            {
                _itemList.Insert(i, itemLog);
                insertOk = true;
                break;
            }
        }
        if (!insertOk)
        {
            _itemList.Add(itemLog);
        }
    }
}

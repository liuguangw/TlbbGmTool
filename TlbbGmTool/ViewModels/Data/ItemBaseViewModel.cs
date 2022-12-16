using liuguang.TlbbGmTool.Models;

namespace liuguang.TlbbGmTool.ViewModels.Data;
public class ItemBaseViewModel
{
    private ItemBase _itemBaseInfo;
    public int ItemBaseId => _itemBaseInfo.Id;
    public string ItemName => _itemBaseInfo.Name;
    public string ItemShortTypeString => _itemBaseInfo.ShortTypeString;
    public string ItemDescription => _itemBaseInfo.Description;
    public byte ItemLevel => _itemBaseInfo.Level;
    public byte ItemMaxSize => _itemBaseInfo.MaxSize;
    public int ItemClass => _itemBaseInfo.TClass;
    public int ItemType => _itemBaseInfo.TType;
    public byte RulerId => _itemBaseInfo.RulerId;
    public ItemBase BaseInfo => _itemBaseInfo;

    public ItemBaseViewModel(ItemBase itemBaseInfo)
    {
        _itemBaseInfo = itemBaseInfo;
    }
}

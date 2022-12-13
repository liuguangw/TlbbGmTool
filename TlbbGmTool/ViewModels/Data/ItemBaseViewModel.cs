using liuguang.TlbbGmTool.Models;

namespace liuguang.TlbbGmTool.ViewModels.Data;
public class ItemBaseViewModel
{
    private ItemBase _itemBaseInfo;
    public int ItemBaseId => _itemBaseInfo.Id;
    public string ItemName => _itemBaseInfo.Name;
    public string ItemShortTypeString => _itemBaseInfo.ShortTypeString;
    public string ItemDescription => _itemBaseInfo.Description;
    public int ItemLevel => _itemBaseInfo.Level;
    public int ItemMaxSize =>_itemBaseInfo.MaxSize;
    public int ItemClass => _itemBaseInfo.TClass;
    public int ItemType => _itemBaseInfo.TType;
    public int EquipVisualId => _itemBaseInfo.EquipVisual;

    public ItemBaseViewModel(ItemBase itemBaseInfo)
    {
        _itemBaseInfo = itemBaseInfo;
    }
}

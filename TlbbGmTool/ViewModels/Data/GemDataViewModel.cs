using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.Services;

namespace liuguang.TlbbGmTool.ViewModels.Data;
/// <summary>
/// 宝石数据 class=5
/// </summary>
public class GemDataViewModel : NotifyBase
{
    #region Fields
    private int _itemBaseId;
    #endregion

    #region Properties
    public int ItemBaseId
    {
        get => _itemBaseId;
        set
        {
            _itemBaseId = value;
            RaisePropertyChanged(nameof(ItemName));
        }
    }
    public string ItemName => ItemService.ParseItemName(_itemBaseId);
    #endregion

    #region ItemFields
    private byte _rulerId;
    private uint _basePrice;
    private byte _attrType = 0;
    private ushort _attrValue = 0;
    #endregion
    #region ItemProperties
    public byte RulerId { get => _rulerId; set => _rulerId = value; }
    public uint BasePrice { get => _basePrice; set => _basePrice = value; }
    public byte AttrType { get => _attrType; set => _attrType = value; }
    public ushort AttrValue { get => _attrValue; set => _attrValue = value; }
    #endregion
}

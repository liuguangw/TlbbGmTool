using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.Services;

namespace liuguang.TlbbGmTool.ViewModels.Data;
/// <summary>
/// 物品数据 class=2/3/4
/// </summary>
public class CommonItemDataViewModel : NotifyBase
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
    private bool _cosSelf = true;
    private uint _basePrice;
    private byte _maxSize = 0xFF;
    private byte _level;
    private int _reqSkill = -1;
    private byte _reqSkillLevel = 0xFF;
    private int _scriptID = -1;
    private int _skillID = -1;
    private byte _targetType = 0xFF;
    private byte _bindStatus;
    private byte _count = 1;
    private byte[] _itemParams = new byte[3 * 4];
    #endregion
    #region ItemProperties
    public byte RulerId { get => _rulerId; set => _rulerId = value; }
    public bool CosSelf { get => _cosSelf; set => _cosSelf = value; }
    public uint BasePrice { get => _basePrice; set => _basePrice = value; }
    public byte MaxSize { get => _maxSize; set => SetProperty(ref _maxSize, value); }
    public byte Level { get => _level; set => _level = value; }
    public int ReqSkill { get => _reqSkill; set => _reqSkill = value; }
    public byte ReqSkillLevel { get => _reqSkillLevel; set => _reqSkillLevel = value; }
    public int ScriptID { get => _scriptID; set => _scriptID = value; }
    public int SkillID { get => _skillID; set => _skillID = value; }
    public byte TargetType { get => _targetType; set => _targetType = value; }
    public byte BindStatus { get => _bindStatus; set => _bindStatus = value; }
    public byte Count { get => _count; set => SetProperty(ref _count, value); }
    public byte[] ItemParams { get => _itemParams; set => _itemParams = value; }
    #endregion
}

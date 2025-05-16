using System;
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
    private int _itemParams0 = 0;
    private int _itemParams1 = 0;
    private int _itemParams2 = 0;
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
    public int ItemParams0
    {
        get => _itemParams0;
        set
        {
            if (SetProperty(ref _itemParams0, value))
            {
                RaisePropertyChanged(nameof(MapLevel));
                RaisePropertyChanged(nameof(SceneID));
                RaisePropertyChanged(nameof(PosX));
            }
        }
    }
    public int ItemParams1
    {
        get => _itemParams1;
        set
        {
            if (SetProperty(ref _itemParams1, value))
            {
                RaisePropertyChanged(nameof(PosX));
                RaisePropertyChanged(nameof(PosY));
                RaisePropertyChanged(nameof(MapExtraValue));
            }
        }
    }
    public int ItemParams2 { get => _itemParams2; set => SetProperty(ref _itemParams2, value); }
    public int MapLevel
    {
        get => _itemParams0 & 0xFF;
        set
        {
            //只需要一个字节
            var valueByte = value & 0xFF;
            //置0
            int mask = 0xFF_FFFF << 8;
            int itemParams0 = _itemParams0 & mask;
            itemParams0 |= valueByte;
            ItemParams0 = itemParams0;
        }
    }
    public int SceneID
    {
        get => (_itemParams0 >> 8) & 0xFFFF;
        set
        {
            //只需要两个字节
            var valueShort = value & 0xFFFF;
            //置0
            int mask = (0xFF << 24) | 0xFF;
            int itemParams0 = _itemParams0 & mask;
            itemParams0 |= (valueShort << 8);
            ItemParams0 = itemParams0;
        }
    }
    public int PosX
    {
        get => ((_itemParams0 >> 24) & 0xFF) | ((_itemParams1 & 0xFF) << 8);
        set
        {
            //只需要两个字节
            var lValueByte = value & 0xFF;
            var hValueByte = (value >> 8) & 0xFF;
            //置0
            int mask = 0xFF_FFFF;
            int itemParams0 = _itemParams0 & mask;
            itemParams0 |= (lValueByte << 24);
            ItemParams0 = itemParams0;
            //置0
            mask = 0xFF_FFFF << 8;
            int itemParams1 = _itemParams1 & mask;
            itemParams1 |= hValueByte;
            ItemParams1 = itemParams1;
        }
    }
    public int PosY
    {
        get => (_itemParams1 >> 8) & 0xFFFF;
        set
        {
            //只需要两个字节
            var valueShort = value & 0xFFFF;
            //置0
            int mask = (0xFF << 24) | 0xFF;
            int itemParams1 = _itemParams1 & mask;
            itemParams1 |= (valueShort << 8);
            ItemParams1 = itemParams1;
        }
    }
    public int MapExtraValue
    {
        get => (_itemParams1 >> 24) & 0xFF;
        set
        {
            //只需要一个字节
            var valueByte = value & 0xFF;
            //置0
            int mask = 0xFF_FFFF;
            int itemParams1 = _itemParams1 & mask;
            itemParams1 |= (valueByte << 24);
            ItemParams1 = itemParams1;
        }
    }
    #endregion
}

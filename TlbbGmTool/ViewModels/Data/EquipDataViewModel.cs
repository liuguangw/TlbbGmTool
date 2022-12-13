using liuguang.TlbbGmTool.Common;

namespace liuguang.TlbbGmTool.ViewModels.Data;
public class EquipDataViewModel : NotifyBase
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
            RaisePropertyChanged(nameof(EquipName));
        }
    }
    public string EquipName => ParseItemName(_itemBaseId);
    public int EquipPoint => ParseItemEquipPoint(_itemBaseId);
    #endregion

    #region EquipFields
    private byte _rulerId;
    private int[] _gemIds = new int[4];
    private byte _bindStatus;
    private byte _maxDurPoint;
    private byte _gemMaxCount;
    private byte _faileTimes;
    private byte _curDurPoint;
    private ushort _curDamagePoint;
    private byte _attrCount;
    private byte _stoneCount;
    private byte _enhanceLevel;
    private byte _adMagic;
    private ushort _adMagicValue;
    private byte[] _aptitude = new byte[6];
    private byte _starCount;
    private ushort _visualId;
    private int[] _attrs = new int[2];
    private byte _hiddenValue;
    #endregion

    #region EquipProperties
    public byte RulerId { get => _rulerId; set => _rulerId = value; }
    public int Gem0
    {
        get => _gemIds[0];
        set
        {
            _gemIds[0] = value;
            RaisePropertyChanged(nameof(Gem0Text));
        }
    }
    public string Gem0Text => ParseGemName(_gemIds[0]);
    public int Gem1
    {
        get => _gemIds[1];
        set
        {
            _gemIds[1] = value;
            RaisePropertyChanged(nameof(Gem1Text));
        }
    }
    public string Gem1Text => ParseGemName(_gemIds[1]);
    public int Gem2
    {
        get => _gemIds[2];
        set
        {
            _gemIds[2] = value;
            RaisePropertyChanged(nameof(Gem2Text));
        }
    }
    public string Gem2Text => ParseGemName(_gemIds[2]);
    public int Gem3
    {
        get => _gemIds[3];
        set
        {
            _gemIds[3] = value;
            RaisePropertyChanged(nameof(Gem3Text));
        }
    }
    public string Gem3Text => ParseGemName(_gemIds[3]);
    public byte BindStatus
    {
        get => _bindStatus;
        set
        {
            _bindStatus = value;
            RaisePropertyChanged(nameof(IsBind));
            RaisePropertyChanged(nameof(IsIden));
            RaisePropertyChanged(nameof(IsAptitudeIden));
            RaisePropertyChanged(nameof(IsEngraved));
            RaisePropertyChanged(nameof(IsPLock));
        }
    }
    public byte MaxDurPoint { get => _maxDurPoint; set => _maxDurPoint = value; }
    public byte GemMaxCount
    {
        get => _gemMaxCount;
        set
        {
            if (SetProperty(ref _gemMaxCount, value))
            {
                //嵌入的宝石数量不能比孔数大
                if (value <= 3)
                {
                    Gem3 = 0;
                }
                if (value <= 2)
                {
                    Gem2 = 0;
                }
                if (value <= 1)
                {
                    Gem1 = 0;
                }
                if (value == 0)
                {
                    Gem0 = 0;
                }

            }
        }
    }
    public byte FaileTimes { get => _faileTimes; set => _faileTimes = value; }
    public byte CurDurPoint { get => _curDurPoint; set => _curDurPoint = value; }
    public ushort CurDamagePoint { get => _curDamagePoint; set => _curDamagePoint = value; }
    public byte AttrCount { get => _attrCount; set => _attrCount = value; }
    public byte StoneCount { get => _stoneCount; set => _stoneCount = value; }
    public byte EnhanceLevel { get => _enhanceLevel; set => SetProperty(ref _enhanceLevel, value); }
    public byte AdMagic { get => _adMagic; set => _adMagic = value; }
    public ushort AdMagicValue { get => _adMagicValue; set => _adMagicValue = value; }

    public byte Aptitude0 { get => _aptitude[0]; set => SetProperty(ref _aptitude[0], value); }
    public byte Aptitude1 { get => _aptitude[1]; set => SetProperty(ref _aptitude[1], value); }
    public byte Aptitude2 { get => _aptitude[2]; set => SetProperty(ref _aptitude[2], value); }
    public byte Aptitude3 { get => _aptitude[3]; set => SetProperty(ref _aptitude[3], value); }
    public byte Aptitude4 { get => _aptitude[4]; set => SetProperty(ref _aptitude[4], value); }
    public byte Aptitude5 { get => _aptitude[5]; set => SetProperty(ref _aptitude[5], value); }

    public byte StarCount { get => _starCount; set => SetProperty(ref _starCount, value); }

    public ushort VisualId
    {
        get => _visualId;
        set
        {
            if (SetProperty(ref _visualId, value))
            {
                RaisePropertyChanged(nameof(VisualText));
            }
        }
    }
    public string VisualText => ParseItemVisual(_visualId);
    public int Attr0
    {
        get => _attrs[0];
        set
        {
            _attrs[0] = value;
            RaisePropertyChanged(nameof(Attr0Tip));
        }
    }
    public string Attr0Tip => CalcAttrTip(_attrs[0]);
    public int Attr1
    {
        get => _attrs[1];
        set
        {
            _attrs[1] = value;
            RaisePropertyChanged(nameof(Attr1Tip));
        }
    }
    public string Attr1Tip => CalcAttrTip(_attrs[1]);
    public byte HiddenValue { get => _hiddenValue; set => SetProperty(ref _hiddenValue, value); }
    #endregion

    #region StatusProperties
    /// <summary>
    /// 已绑定
    /// </summary>
    public bool IsBind
    {
        get => (_bindStatus & 0b0000_0001) != 0;
        set
        {
            if (value)
            {
                _bindStatus |= 0b0000_0001;
            }
            else
            {
                _bindStatus &= 0b1111_1110;
            }
        }
    }
    /// <summary>
    /// 已鉴定
    /// </summary>
    public bool IsIden
    {
        get => (_bindStatus & 0b0000_0010) != 0;
        set
        {
            if (value)
            {
                _bindStatus |= 0b0000_0010;
            }
            else
            {
                _bindStatus &= 0b1111_1101;
            }
        }
    }
    /// <summary>
    /// 二级密码锁定
    /// </summary>
    public bool IsPLock
    {
        get => (_bindStatus & 0b0000_0100) != 0;
        set
        {
            if (value)
            {
                _bindStatus |= 0b0000_0100;
            }
            else
            {
                _bindStatus &= 0b1111_1011;
            }
        }
    }
    /// <summary>
    /// 是否有制作者
    /// </summary>
    public bool HasCreator
    {
        get => (_bindStatus & 0b0001_0000) != 0;
        set
        {
            if (value)
            {
                _bindStatus |= 0b0001_0000;
            }
            else
            {
                _bindStatus &= 0b1110_1111;
            }
        }
    }
    /// <summary>
    /// 是否已经鉴定过资质
    /// </summary>
    public bool IsAptitudeIden
    {
        get => (_bindStatus & 0b0010_0000) != 0;
        set
        {
            if (value)
            {
                _bindStatus |= 0b0010_0000;
            }
            else
            {
                _bindStatus &= 0b1101_1111;
            }
        }
    }
    /// <summary>
    /// 是否已经刻铭
    /// </summary>
    public bool IsEngraved
    {
        get => (_bindStatus & 0b0100_0000) != 0;
        set
        {
            if (value)
            {
                _bindStatus |= 0b0100_0000;
            }
            else
            {
                _bindStatus &= 0b1011_1111;
            }
        }
    }
    #endregion
    private static string ParseItemName(int itemBaseId)
    {
        if (SharedData.ItemBaseMap.TryGetValue(itemBaseId, out var itemBaseInfo))
        {
            return $"{itemBaseInfo.Name}(ID: {itemBaseId})";
        }
        return $"未知物品(ID: {itemBaseId})";
    }
    private static int ParseItemEquipPoint(int itemBaseId)
    {
        if (SharedData.ItemBaseMap.TryGetValue(itemBaseId, out var itemBaseInfo))
        {
            return itemBaseInfo.EquipPoint;
        }
        return 0;
    }
    private static string ParseGemName(int gemId)
    {
        if (gemId == 0)
        {
            return "无";
        }
        return ParseItemName(gemId);
    }

    /// <summary>
    /// 根据外观id，计算物品id
    /// </summary>
    /// <param name="visualId"></param>
    /// <returns></returns>
    public int? ParseVisualItemId(int visualId)
    {
        //判断外形id是否是装备的默认外形id
        if (SharedData.ItemBaseMap.TryGetValue(_itemBaseId, out var itemBaseInfo))
        {
            if (itemBaseInfo.EquipVisual == visualId)
            {
                return _itemBaseId;
            }
        }
        //遍历搜索
        foreach (var tBaseInfo in SharedData.ItemBaseMap.Values)
        {
            if (tBaseInfo.EquipVisual == visualId)
            {
                return tBaseInfo.Id;
            }
        }
        return null;
    }

    private string ParseItemVisual(int visualId)
    {
        //判断外形id是否是装备的默认外形id
        if (SharedData.ItemBaseMap.TryGetValue(_itemBaseId, out var itemBaseInfo))
        {
            if (itemBaseInfo.EquipVisual == visualId)
            {
                return $"{itemBaseInfo.Name}(ID: {itemBaseInfo.Id})";
            }
        }
        //遍历搜索
        foreach (var tBaseInfo in SharedData.ItemBaseMap.Values)
        {
            if (tBaseInfo.EquipVisual == visualId)
            {
                return $"{tBaseInfo.Name}(ID: {tBaseInfo.Id})";
            }

        }
        return $"未知外形(visual ID: {visualId})";
    }
    private static int GetAttrCountFromNumber(int attrValue)
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

        return attrCount;
    }

    private static string CalcAttrTip(int attrValue)
    {
        return $"已选择{GetAttrCountFromNumber(attrValue)}种属性";
    }
    /// <summary>
    /// 重新计算嵌入的宝石个数
    /// </summary>
    public void ReloadStoneCount()
    {
        byte stoneCount = 0;
        foreach (var gemId in _gemIds)
        {
            if (gemId != 0)
            {
                stoneCount++;
            }
        }
        _stoneCount = stoneCount;
    }
    /// <summary>
    /// 重新计算属性条数
    /// </summary>
    public void ReloadAttrCount()
    {
        var attrCount = GetAttrCountFromNumber(_attrs[0]) + GetAttrCountFromNumber(_attrs[1]);
        _attrCount = (byte)attrCount;
    }
}

using liuguang.TlbbGmTool.Common;
using System;
using System.Collections.Generic;

namespace liuguang.TlbbGmTool.ViewModels;
public class EquipEditorViewModel : ViewModelBase
{
    #region Fields
    private bool _isSaving = false;
    private ItemLogViewModel? _inputItemLog;
    private ItemLogViewModel _itemLog = new(new());
    /// <summary>
    /// 数据库连接
    /// </summary>
    public DbConnection? Connection;
    #endregion
    #region Properties
    public bool IsSaving
    {
        get => _isSaving;
        set
        {
            if (SetProperty(ref _isSaving, value))
            {
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
    }
    public string WindowTitle
    {
        get
        {
            if (_inputItemLog is null)
            {
                return "发放装备";
            }
            return "修改装备 " + EquipName;
        }
    }
    public ItemLogViewModel ItemLog
    {
        set
        {
            _inputItemLog = value;
            _itemLog.ItemBaseId = value.ItemBaseId;
            _itemLog.PData = value.PData;
            RaiseDataChanged();
        }
    }
    /// <summary>
    /// 星级选择
    /// </summary>
    public List<ComboBoxNode<byte>> StarSection { get; } = new();
    /// <summary>
    /// 孔数选择
    /// </summary>
    public List<ComboBoxNode<byte>> GemMaxCountSection { get; } = new();
    public string EquipName => ParseItemName(_itemLog.ItemBaseId);
    public string VisualText => ParseItemVisual(VisualId);
    public string Gem0Text => ParseGemName(Gem0);
    public string Gem1Text => ParseGemName(Gem1);
    public string Gem2Text => ParseGemName(Gem2);
    public string Gem3Text => ParseGemName(Gem3);

    public string Attr1Tip => CalcAttrTip(Attr1);
    public string Attr2Tip => CalcAttrTip(Attr2);
    #endregion
    #region EquipData
    public byte RulerId
    {
        get => _itemLog.PData[0];
        set
        {
            _itemLog.PData[0] = value;
        }
    }
    public int Gem0
    {
        get => ReadInt(_itemLog.PData, 2);
        set
        {
            WriteData(_itemLog.PData, 2, value);
            RaisePropertyChanged(nameof(Gem0Text));
        }
    }
    public int Gem1
    {
        get => ReadInt(_itemLog.PData, 6);
        set
        {
            WriteData(_itemLog.PData, 6, value);
            RaisePropertyChanged(nameof(Gem1Text));
        }
    }
    public int Gem2
    {
        get => ReadInt(_itemLog.PData, 10);
        set
        {
            WriteData(_itemLog.PData, 10, value);
            RaisePropertyChanged(nameof(Gem2Text));
        }
    }
    /// <summary>
    /// 已绑定
    /// </summary>
    public bool IsBind
    {
        get => (_itemLog.PData[14] & 0b0000_0001) != 0;
        set
        {
            if (value)
            {
                _itemLog.PData[14] |= 0b0000_0001;
            }
            else
            {
                _itemLog.PData[14] &= 0b1111_1110;
            }
        }
    }
    /// <summary>
    /// 已鉴定
    /// </summary>
    public bool IsIden
    {
        get => (_itemLog.PData[14] & 0b0000_0010) != 0;
        set
        {
            if (value)
            {
                _itemLog.PData[14] |= 0b0000_0010;
            }
            else
            {
                _itemLog.PData[14] &= 0b1111_1101;
            }
        }
    }
    /// <summary>
    /// 二级密码锁定
    /// </summary>
    public bool IsPLock
    {
        get => (_itemLog.PData[14] & 0b0000_0100) != 0;
        set
        {
            if (value)
            {
                _itemLog.PData[14] |= 0b0000_0100;
            }
            else
            {
                _itemLog.PData[14] &= 0b1111_1011;
            }
        }
    }
    /// <summary>
    /// 是否有制作者
    /// </summary>
    public bool HasCreator
    {
        get => (_itemLog.PData[14] & 0b0001_0000) != 0;
        set
        {
            if (value)
            {
                _itemLog.PData[14] |= 0b0001_0000;
            }
            else
            {
                _itemLog.PData[14] &= 0b1110_1111;
            }
        }
    }
    /// <summary>
    /// 是否已经鉴定过资质
    /// </summary>
    public bool IsAptitudeIden
    {
        get => (_itemLog.PData[14] & 0b0010_0000) != 0;
        set
        {
            if (value)
            {
                _itemLog.PData[14] |= 0b0010_0000;
            }
            else
            {
                _itemLog.PData[14] &= 0b1101_1111;
            }
        }
    }
    /// <summary>
    /// 是否已经刻铭
    /// </summary>
    public bool IsEngraved
    {
        get => (_itemLog.PData[14] & 0b0100_0000) != 0;
        set
        {
            if (value)
            {
                _itemLog.PData[14] |= 0b0100_0000;
            }
            else
            {
                _itemLog.PData[14] &= 0b1011_1111;
            }
        }
    }
    public byte MaxDurPoint
    {
        get => _itemLog.PData[15];
        set
        {
            _itemLog.PData[15] = value;
        }
    }
    public byte GemMaxCount
    {
        get => _itemLog.PData[16];
        set
        {
            _itemLog.PData[16] = value;
            //
            SelectGem0Command.RaiseCanExecuteChanged();
            SelectGem1Command.RaiseCanExecuteChanged();
            SelectGem2Command.RaiseCanExecuteChanged();
            SelectGem3Command.RaiseCanExecuteChanged();
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
    public byte FaileTimes
    {
        get => _itemLog.PData[17];
        set
        {
            _itemLog.PData[17] = value;
        }
    }
    public byte CurDurPoint
    {
        get => _itemLog.PData[18];
        set
        {
            _itemLog.PData[18] = value;
        }
    }
    public ushort CurDamagePoint
    {
        get => ReadUshort(_itemLog.PData, 19);
        set => WriteData(_itemLog.PData, 19, value);
    }
    public byte AttrCount
    {
        get => _itemLog.PData[21];
        set
        {
            _itemLog.PData[21] = value;
        }
    }
    public byte StoneCount
    {
        get => _itemLog.PData[22];
        set
        {
            _itemLog.PData[22] = value;
        }
    }
    public byte EnhanceLevel
    {
        get => _itemLog.PData[23];
        set
        {
            _itemLog.PData[23] = value;
            RaisePropertyChanged();
        }
    }
    public byte AdMagic
    {
        get => _itemLog.PData[24];
        set
        {
            _itemLog.PData[24] = value;
        }
    }
    public ushort AdMagicValue
    {
        get => ReadUshort(_itemLog.PData, 25);
        set => WriteData(_itemLog.PData, 25, value);
    }
    public byte Aptitude0
    {
        get => _itemLog.PData[27];
        set
        {
            _itemLog.PData[27] = value;
            RaisePropertyChanged();
        }
    }
    public byte Aptitude1
    {
        get => _itemLog.PData[28];
        set
        {
            _itemLog.PData[28] = value;
            RaisePropertyChanged();
        }
    }
    public byte Aptitude2
    {
        get => _itemLog.PData[29];
        set
        {
            _itemLog.PData[29] = value;
            RaisePropertyChanged();
        }
    }
    public byte Aptitude3
    {
        get => _itemLog.PData[30];
        set
        {
            _itemLog.PData[30] = value;
            RaisePropertyChanged();
        }
    }
    public byte Aptitude4
    {
        get => _itemLog.PData[31];
        set
        {
            _itemLog.PData[31] = value;
            RaisePropertyChanged();
        }
    }
    public byte Aptitude5
    {
        get => _itemLog.PData[32];
        set
        {
            _itemLog.PData[32] = value;
            RaisePropertyChanged();
        }
    }
    public byte StarCount
    {
        get => _itemLog.PData[33];
        set
        {
            _itemLog.PData[33] = value;
        }
    }
    public ushort VisualId
    {
        get => ReadUshort(_itemLog.PData, 34);
        set => WriteData(_itemLog.PData, 34, value);
    }

    public int Attr1
    {
        get => ReadInt(_itemLog.PData, 36);
        set
        {
            WriteData(_itemLog.PData, 36, value);
            RaisePropertyChanged(nameof(Attr1Tip));
        }
    }

    public int Attr2
    {
        get => ReadInt(_itemLog.PData, 40);
        set
        {
            WriteData(_itemLog.PData, 40, value);
            RaisePropertyChanged(nameof(Attr1Tip));
        }
    }
    public byte HiddenValue
    {
        get => _itemLog.PData[44];
        set
        {
            _itemLog.PData[44] = value;
            RaisePropertyChanged();
        }
    }
    public int Gem3
    {
        get => ReadInt(_itemLog.PData, 61);
        set
        {
            WriteData(_itemLog.PData, 61, value);
            RaisePropertyChanged(nameof(Gem3Text));
        }
    }

    #endregion
    #region Commands
    public Command SelectEquipCommand { get; }
    public Command SelectVisualCommand { get; }
    public Command SelectGem0Command { get; }
    public Command SelectGem1Command { get; }
    public Command SelectGem2Command { get; }
    public Command SelectGem3Command { get; }
    public Command SelectAttrCommand { get; }
    public Command SaveCommand { get; }
    #endregion

    public EquipEditorViewModel()
    {
        SelectEquipCommand = new(() => { });
        SelectVisualCommand = new(() => { });
        SelectGem0Command = new(() => { }, () => GemMaxCount > 0);
        SelectGem1Command = new(() => { }, () => GemMaxCount > 1);
        SelectGem2Command = new(() => { }, () => GemMaxCount > 2);
        SelectGem3Command = new(() => { }, () => GemMaxCount > 3);
        SelectAttrCommand = new(() => { });
        SaveCommand = new(SaveItem, () => !_isSaving);
        for (byte i = 0; i <= 9; i++)
        {
            StarSection.Add(new($"{i}星", i));
        }
        for (byte i = 0; i <= 4; i++)
        {
            GemMaxCountSection.Add(new($"{i}孔", i));
        }
    }

    private void RaiseDataChanged()
    {
        RaisePropertyChanged(nameof(WindowTitle));
        RaisePropertyChanged(nameof(EquipName));
        RaisePropertyChanged(nameof(StarCount));
        RaisePropertyChanged(nameof(GemMaxCount));
        //
        SelectGem0Command.RaiseCanExecuteChanged();
        SelectGem1Command.RaiseCanExecuteChanged();
        SelectGem2Command.RaiseCanExecuteChanged();
        SelectGem3Command.RaiseCanExecuteChanged();
        //
        RaisePropertyChanged(nameof(VisualText));

        RaisePropertyChanged(nameof(Gem0Text));
        RaisePropertyChanged(nameof(Gem1Text));
        RaisePropertyChanged(nameof(Gem2Text));
        RaisePropertyChanged(nameof(Gem3Text));

        RaisePropertyChanged(nameof(Attr1Tip));
        RaisePropertyChanged(nameof(Attr2Tip));

        RaisePropertyChanged(nameof(Aptitude0));
        RaisePropertyChanged(nameof(Aptitude1));
        RaisePropertyChanged(nameof(Aptitude2));
        RaisePropertyChanged(nameof(Aptitude3));
        RaisePropertyChanged(nameof(Aptitude4));
        RaisePropertyChanged(nameof(Aptitude5));

        RaisePropertyChanged(nameof(EnhanceLevel));
        RaisePropertyChanged(nameof(HiddenValue));

        RaisePropertyChanged(nameof(IsBind));
        RaisePropertyChanged(nameof(IsIden));
        RaisePropertyChanged(nameof(IsAptitudeIden));
        RaisePropertyChanged(nameof(IsEngraved));
        RaisePropertyChanged(nameof(IsPLock));
    }

    private byte[] LoadBuff(byte[] src, int offset, int length)
    {
        var buff = new byte[length];
        Array.Copy(src, offset, buff, 0, length);
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(buff);
        }
        return buff;
    }
    private int ReadInt(byte[] src, int offset)
    {
        var buff = LoadBuff(src, offset, 4);
        return BitConverter.ToInt32(buff, 0);
    }
    private ushort ReadUshort(byte[] src, int offset)
    {
        var buff = LoadBuff(src, offset, 2);
        return BitConverter.ToUInt16(buff, 0);
    }
    private void WriteData(byte[] dist, int offset, int value)
    {
        var buff = BitConverter.GetBytes(value);
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(buff);
        }
        Array.Copy(buff, 0, dist, offset, buff.Length);
    }
    private void WriteData(byte[] dist, int offset, ushort value)
    {
        var buff = BitConverter.GetBytes(value);
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(buff);
        }
        Array.Copy(buff, 0, dist, offset, buff.Length);
    }


    private string ParseItemName(int itemBaseId)
    {
        if (SharedData.ItemBaseMap.TryGetValue(itemBaseId, out var itemBaseInfo))
        {
            return $"{itemBaseInfo.Name}(ID: {itemBaseId})";
        }
        return $"未知物品(ID: {itemBaseId})";
    }
    private string ParseGemName(int gemId)
    {
        if (gemId == 0)
        {
            return "无";
        }
        return ParseItemName(gemId);
    }

    public string ParseItemVisual(int visualId)
    {
        //判断外形id是否是装备的默认外形id
        if (SharedData.ItemBaseMap.TryGetValue(_itemLog.ItemBaseId, out var itemBaseInfo))
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

    private void SaveItem()
    {
        ShowMessage("debug", $"gem2={Gem2}");
    }
}

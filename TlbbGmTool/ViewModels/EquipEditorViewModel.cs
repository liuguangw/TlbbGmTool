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
    /// <summary>
    /// 规则编号
    /// </summary>
    private byte _rulerId;
    /// <summary>
    /// 镶嵌的宝石id
    /// </summary>
    private int[] _gemIds = new int[4];
    /// <summary>
    /// 状态
    /// </summary>
    private byte _bindStatus;
    /// <summary>
    /// 最大耐久值
    /// </summary>
    private byte _maxDurPoint;
    /// <summary>
    /// 孔数
    /// </summary>
    private byte _gemMaxCount;
    /// <summary>
    /// 失败次数
    /// </summary>
    private byte _faileTimes;
    /// <summary>
    /// 当前耐久值
    /// </summary>
    private byte _curDurPoint;
    /// <summary>
    /// 当前损伤度
    /// </summary>
    private ushort _curDamagePoint;
    /// <summary>
    /// 属性条数
    /// </summary>
    private byte _attrCount;
    /// <summary>
    /// 已嵌入的宝石数量
    /// </summary>
    private byte _stoneCount;
    /// <summary>
    /// 强化等级
    /// </summary>
    private byte _enhanceLevel;
    private byte _adMagic;
    private ushort _adMagicValue;
    /// <summary>
    /// 资质
    /// </summary>
    private byte[] _aptitude = new byte[6];
    /// <summary>
    /// 星级
    /// </summary>
    private byte _starCount;
    /// <summary>
    /// 外观
    /// </summary>
    private ushort _visualId;
    /// <summary>
    /// 攻击属性
    /// </summary>
    private int _attr1;
    /// <summary>
    /// 防御属性
    /// </summary>
    private int _attr2;
    /// <summary>
    /// 浮动值
    /// </summary>
    private byte _hiddenValue;
    #endregion
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
    #region Properties
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
            _itemLog.Creator = value.Creator;
            LoadEquipData(value.PData);
            RaisePropertyChanged(nameof(WindowTitle));
            RaisePropertyChanged(nameof(EquipName));
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
    public string EquipName
    {
        get => ParseItemName(_itemLog.ItemBaseId);
    }
    public byte StarCount
    {
        get => _starCount; set => SetProperty(ref _starCount, value);
    }
    public byte GemMaxCount
    {
        get => _gemMaxCount;
        set
        {
            if (SetProperty(ref _gemMaxCount, value))
            {
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
    }
    public ushort VisualId
    {
        get => _visualId; set
        {
            if (SetProperty(ref _visualId, value))
            {
                RaisePropertyChanged(nameof(VisualText));
            }
        }
    }
    public string VisualText => ParseItemVisual(_visualId);
    private int Gem0
    {
        get => _gemIds[0];
        set
        {
            if (SetProperty(ref _gemIds[0], value))
            {
                RaisePropertyChanged(nameof(Gem0Text));
            }
        }
    }
    public string Gem0Text => ParseGemName(0);
    private int Gem1
    {
        get => _gemIds[1];
        set
        {
            if (SetProperty(ref _gemIds[1], value))
            {
                RaisePropertyChanged(nameof(Gem1Text));
            }
        }
    }
    public string Gem1Text => ParseGemName(1);
    private int Gem2
    {
        get => _gemIds[2];
        set
        {
            if (SetProperty(ref _gemIds[2], value))
            {
                RaisePropertyChanged(nameof(Gem2Text));
            }
        }
    }
    public string Gem2Text => ParseGemName(2);
    private int Gem3
    {
        get => _gemIds[3];
        set
        {
            if (SetProperty(ref _gemIds[3], value))
            {
                RaisePropertyChanged(nameof(Gem3Text));
            }
        }
    }
    public string Gem3Text => ParseGemName(3);
    public int Attr1
    {
        set
        {
            if (SetProperty(ref _attr1, value))
            {
                RaisePropertyChanged(nameof(Attr1Tip));
            }
        }
    }

    public string Attr1Tip => CalcAttrTip(_attr1);

    public int Attr2
    {
        set
        {
            if (SetProperty(ref _attr2, value))
            {
                RaisePropertyChanged(nameof(Attr2Tip));
            }
        }
    }

    public string Attr2Tip => CalcAttrTip(_attr2);
    public byte Aptitude0
    {
        get => _aptitude[0]; set => SetProperty(ref _aptitude[0], value);
    }
    public byte Aptitude1
    {
        get => _aptitude[1]; set => SetProperty(ref _aptitude[1], value);
    }
    public byte Aptitude2
    {
        get => _aptitude[2]; set => SetProperty(ref _aptitude[2], value);
    }
    public byte Aptitude3
    {
        get => _aptitude[3]; set => SetProperty(ref _aptitude[3], value);
    }
    public byte Aptitude4
    {
        get => _aptitude[4]; set => SetProperty(ref _aptitude[4], value);
    }
    public byte Aptitude5
    {
        get => _aptitude[5]; set => SetProperty(ref _aptitude[5], value);
    }
    public byte EnhanceLevel
    {
        get => _enhanceLevel; set => SetProperty(ref _enhanceLevel, value);
    }
    public byte HiddenValue
    {
        get => _hiddenValue; set => SetProperty(ref _hiddenValue, value);
    }
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
        SelectGem0Command = new(() => { }, () => _gemMaxCount > 0);
        SelectGem1Command = new(() => { }, () => _gemMaxCount > 1);
        SelectGem2Command = new(() => { }, () => _gemMaxCount > 2);
        SelectGem3Command = new(() => { }, () => _gemMaxCount > 3);
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

    /// <summary>
    /// 从p1-p17这段数据中加载装备信息
    /// </summary>
    /// <param name="pData"></param>
    private void LoadEquipData(byte[] pData)
    {
        int offset = 0;
        var readNextByte = () =>
        {
            var value = pData[offset];
            offset++;
            return value;
        };
        var readNextUshort = () =>
        {
            var value = ReadUshort(pData, offset);
            offset += 2;
            return value;
        };
        var readNextInt = () =>
        {
            var value = ReadInt(pData, offset);
            offset += 4;
            return value;
        };
        //
        _rulerId = readNextByte();
        //跳过固定为0的字节
        offset++;
        //读取前三个宝石id
        for (var i = 0; i < 3; i++)
        {
            _gemIds[i] = readNextInt();
        }
        RaisePropertyChanged(nameof(Gem0Text));
        RaisePropertyChanged(nameof(Gem1Text));
        RaisePropertyChanged(nameof(Gem2Text));
        _bindStatus = readNextByte();
        RaisePropertyChanged(nameof(IsBind));
        RaisePropertyChanged(nameof(IsIden));
        RaisePropertyChanged(nameof(IsAptitudeIden));
        RaisePropertyChanged(nameof(IsEngraved));
        RaisePropertyChanged(nameof(IsPLock));
        _maxDurPoint = readNextByte();
        GemMaxCount = readNextByte();
        _faileTimes = readNextByte();
        _curDurPoint = readNextByte();
        _curDamagePoint = readNextUshort();
        _attrCount = readNextByte();
        _stoneCount = readNextByte();
        EnhanceLevel = readNextByte();
        _adMagic = readNextByte();
        _adMagicValue = readNextUshort();
        //资质
        for (var i = 0; i < _aptitude.Length; i++)
        {
            _aptitude[i] = readNextByte();
        }
        RaisePropertyChanged(nameof(Aptitude0));
        RaisePropertyChanged(nameof(Aptitude1));
        RaisePropertyChanged(nameof(Aptitude2));
        RaisePropertyChanged(nameof(Aptitude3));
        RaisePropertyChanged(nameof(Aptitude4));
        RaisePropertyChanged(nameof(Aptitude5));
        StarCount = readNextByte();
        VisualId = readNextUshort();
        Attr1 = readNextInt();
        Attr2 = readNextInt();
        HiddenValue = readNextByte();
        offset += 16;
        //最后一个宝石信息
        _gemIds[3] = readNextInt();
        RaisePropertyChanged(nameof(Gem3Text));
    }
    private void WriteEquipData(byte[] pOut)
    {
        int offset = 0;
        var writeNextByte = (byte value) =>
        {
            pOut[offset] = value;
            offset++;
        };
        var writeNextUshort = (ushort value) =>
        {
            WriteData(pOut, offset, value);
            offset += 2;
        };
        var writeNextInt = (int value) =>
        {
            WriteData(pOut, offset, value);
            offset += 4;
        };
        writeNextByte(_rulerId);
        //跳过固定为0的字节
        offset += 1;
        //前三个宝石id
        for (var i = 0; i < 3; i++)
        {
            writeNextInt(_gemIds[i]);
        }
        writeNextByte(_bindStatus);
        writeNextByte(_maxDurPoint);
        writeNextByte(_gemMaxCount);
        writeNextByte(_faileTimes);
        writeNextByte(_curDurPoint);
        writeNextUshort(_curDamagePoint);
        //计算属性条数
        var attrCount= GetAttrCountFromNumber(_attr1)+GetAttrCountFromNumber(_attr2);
        _attrCount = (byte)attrCount;
        writeNextByte(_attrCount);
        //计算宝石的数量
        byte stoneCount = 0;
        for (var i = 0; i < _gemIds.Length; i++)
        {
            if (_gemIds[i] != 0)
            {
                stoneCount++;
            }
        }
        _stoneCount = stoneCount;
        writeNextByte(_stoneCount);
        writeNextByte(_enhanceLevel);
        writeNextByte(_adMagic);
        writeNextUshort(_adMagicValue);
        //资质
        for (var i = 0; i < _aptitude.Length; i++)
        {
            writeNextByte(_aptitude[i]);
        }
        writeNextByte(_starCount);
        writeNextUshort(_visualId);
        writeNextInt(_attr1);
        writeNextInt(_attr2);
        writeNextByte(_hiddenValue);
        offset += 16;
        //最后一个宝石信息
        writeNextInt(_gemIds[3]);
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
    private string ParseGemName(int gemIndex)
    {
        var gemId = _gemIds[gemIndex];
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
        ShowErrorMessage("todo", "todo");
    }
}

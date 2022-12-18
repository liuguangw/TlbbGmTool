using liuguang.TlbbGmTool.Common;

namespace liuguang.TlbbGmTool.ViewModels.Data;
/// <summary>
/// 暗器部分数据结构
/// </summary>
public class DarkDataViewModel : NotifyBase
{
    #region ItemFields
    private int _nowExp;
    private short[] _impactIds = new short[3];
    private short[] _appendAttrs = new short[5];
    private short _useTimes;
    private short _quality;
    private byte _level;
    #endregion
    #region ItemProperties
    public int NowExp { get => _nowExp; set => SetProperty(ref _nowExp, value); }
    public short[] ImpactIds { get => _impactIds; set => SetProperty(ref _impactIds, value); }

    public short Impact0
    {
        get => _impactIds[0];
        set
        {
            if (SetProperty(ref _impactIds[0], value))
            {
                RaisePropertyChanged(nameof(Impact0Text));
            }
        }
    }
    public short Impact1
    {
        get => _impactIds[1];
        set
        {
            if (SetProperty(ref _impactIds[1], value))
            {
                RaisePropertyChanged(nameof(Impact1Text));
            }
        }
    }
    public short Impact2
    {
        get => _impactIds[2];
        set
        {
            if (SetProperty(ref _impactIds[2], value))
            {
                RaisePropertyChanged(nameof(Impact2Text));
            }
        }
    }

    public string Impact0Text => ParseDarkImpact(_impactIds[0]);
    public string Impact1Text => ParseDarkImpact(_impactIds[1]);
    public string Impact2Text => ParseDarkImpact(_impactIds[2]);
    public short AppendAttr0 { get => _appendAttrs[0]; set => SetProperty(ref _appendAttrs[0], value); }
    public short AppendAttr1 { get => _appendAttrs[1]; set => SetProperty(ref _appendAttrs[1], value); }
    public short AppendAttr2 { get => _appendAttrs[2]; set => SetProperty(ref _appendAttrs[2], value); }
    public short AppendAttr3 { get => _appendAttrs[3]; set => SetProperty(ref _appendAttrs[3], value); }
    public short AppendAttr4 { get => _appendAttrs[4]; set => SetProperty(ref _appendAttrs[4], value); }
    public short UseTimes { get => _useTimes; set => SetProperty(ref _useTimes, value); }
    public short Quality { get => _quality; set => SetProperty(ref _quality, value); }
    public byte Level { get => _level; set => SetProperty(ref _level, value); }
    #endregion
    private static string ParseDarkImpact(short impactId)
    {
        if (impactId == 0)
        {
            return string.Empty;
        }
        if (SharedData.DarkImpactMap.TryGetValue(impactId, out var impactDescription))
        {
            return impactDescription;
        }
        return string.Empty;
    }
}

using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.Models;

namespace liuguang.TlbbGmTool.ViewModels;

public class XinFaLogViewModel : NotifyBase
{
    #region Fields
    private XinFaLog _xinFaLog;
    #endregion
    #region Properties
    public int Id
    {
        get => _xinFaLog.Id;
        private set => SetProperty(ref _xinFaLog.Id, value);
    }
    public int CharGuid
    {
        get => _xinFaLog.CharGuid;
        private set => SetProperty(ref _xinFaLog.CharGuid, value);
    }

    public int XinFaId
    {
        get => _xinFaLog.XinFaId;
        private set
        {
            if (SetProperty(ref _xinFaLog.XinFaId, value))
            {
                //用于editor中
                RaisePropertyChanged(nameof(XinFaName));
                RaisePropertyChanged(nameof(XinFaDescription));
            }
        }
    }

    public string XinFaName
    {
        get
        {
            if (SharedData.XinFaMap.TryGetValue(_xinFaLog.XinFaId, out var xinFaBaseInfo))
            {
                return xinFaBaseInfo.Name;
            }
            return string.Empty;
        }
    }

    public string XinFaDescription
    {
        get
        {
            if (SharedData.XinFaMap.TryGetValue(_xinFaLog.XinFaId, out var xinFaBaseInfo))
            {
                return xinFaBaseInfo.Description;
            }
            return string.Empty;
        }
    }

    public int XinFaLevel
    {
        get => _xinFaLog.XinFaLevel;
        set => SetProperty(ref _xinFaLog.XinFaLevel, value);
    }
    #endregion

    public XinFaLogViewModel(XinFaLog xinFaLog)
    {
        _xinFaLog = xinFaLog;
    }

    public void CopyFrom(XinFaLogViewModel src)
    {
        Id = src.Id;
        CharGuid = src.CharGuid;
        XinFaId = src.XinFaId;
        XinFaLevel = src.XinFaLevel;
    }
}

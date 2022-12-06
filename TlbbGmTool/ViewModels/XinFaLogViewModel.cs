using System.Collections.Generic;
using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.Models;

namespace liuguang.TlbbGmTool.ViewModels;

public class XinFaLogViewModel : NotifyBase
{
    public static readonly SortedDictionary<int, XinFaBase> XinFaMap = new();
    #region Fields
    private XinFaLog _xinFaLog;
    #endregion
    #region Properties
    public int Id => _xinFaLog.Id;
    public int CharGuid => _xinFaLog.CharGuid;

    public int XinFaId => _xinFaLog.XinFaId;

    public string XinFaName
    {
        get
        {
            if (XinFaMap.TryGetValue(_xinFaLog.XinFaId, out var xinFaBaseInfo))
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
            if (XinFaMap.TryGetValue(_xinFaLog.XinFaId, out var xinFaBaseInfo))
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
}
using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.ViewModels.Data;
using System.ComponentModel;

namespace liuguang.TlbbGmTool.ViewModels;
public class RoleWindowViewModel : ViewModelBase
{
    #region Fields
    private RoleViewModel? _roleInfo;
    /// <summary>
    /// 数据库连接
    /// </summary>
    public DbConnection? Connection;
    #endregion

    #region Properties
    public RoleViewModel? RoleInfo
    {
        get => _roleInfo;
        set
        {
            if (SetProperty(ref _roleInfo, value))
            {
                if (value != null)
                {
                    value.PropertyChanged += RoleInfo_PropertyChanged;
                }
                RaisePropertyChanged(nameof(WindowTitle));
            }
        }
    }

    public string WindowTitle
    {
        get
        {
            if (RoleInfo is null)
            {
                return string.Empty;
            }
            return $"管理 {RoleInfo.CharName}(角色id: {RoleInfo.CharGuid})";
        }
    }

    #endregion

    private void RoleInfo_PropertyChanged(object? sender, PropertyChangedEventArgs evt)
    {
        RoleViewModel? value;
        if (evt.PropertyName == nameof(value.CharName))
        {
            //ShowMessage("debug",$"{evt.PropertyName} changed");
            RaisePropertyChanged(nameof(WindowTitle));
        }
    }
}

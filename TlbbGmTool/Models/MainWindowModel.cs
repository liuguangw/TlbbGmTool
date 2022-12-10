using System.Collections.Generic;
using System.Collections.ObjectModel;
using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.ViewModels;

namespace liuguang.TlbbGmTool.Models;

public class MainWindowModel
{
    /// <summary>
    /// 数据加载状态
    /// </summary>
    public DataStatus DataStatus = DataStatus.NotLoad;

    /// <summary>
    /// 数据库连接
    /// </summary>
    public DbConnection Connection = new();

    /// <summary>
    /// 数据库版本
    /// </summary>
    public string DbVersion = string.Empty;

    /// <summary>
    /// server list
    /// </summary>
    public ObservableCollection<GameServerViewModel> ServerList = new();
}

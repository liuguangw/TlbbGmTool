namespace liuguang.TlbbGmTool.Models;

/// <summary>
/// 数据库连接状态
/// </summary>
public enum DbStatus
{
    //未连接
    NotConnect,
    //正在连接
    Connecting,
    //已连接
    Connected
}